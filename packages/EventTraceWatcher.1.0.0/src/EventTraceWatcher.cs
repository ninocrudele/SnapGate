//------------------------------------------------------------------------------
// Author: Daniel Vasquez-Lopez 2009
//------------------------------------------------------------------------------

#pragma warning disable 1634, 1691

//
// Define this to cache the trace event information by opcode.
// It can be cached by other key part of the trace event for
// scenarios with specific events.
// The reason for this cache is that 'TraceEventInfoWrapper' is
// expensive to construct, but this efficiency may not be required
// in all scenarios. Measure first without the cache.
//
//#define UseTraceEventCache

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Samples.Eventing.Interop;

namespace Microsoft.Samples.Eventing
{
    public enum TraceLevel
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Information = 4,
        Verbose = 5
    }

    public sealed class EventTraceWatcher : IDisposable
    {
        private readonly string loggerName;
        private Guid eventProviderId;
        private bool enabled;
        private volatile bool closing;
        private TraceSafeHandle traceHandle;
        private SessionSafeHandle sessionHandle;
        private EventTraceLogfile logFile;
        private Thread processEventsThread;
        private EventTraceProperties eventTraceProperties;

        // Optional cache, can be cached by opcode or another key.
#if UseTraceEventCache
        private SortedList<byte, TraceEventInfoWrapper> traceEventInfoCache = new SortedList<byte/*opcode*/, TraceEventInfoWrapper>();
#endif

        private delegate void ProcessTraceDelegate(TraceSafeHandle traceHandle);

        public EventTraceWatcher(string loggerName, Guid eventProviderId)
        {
            this.loggerName = loggerName;
            this.eventProviderId = eventProviderId;
            BufferSizeInKiloBytes = 64;
        }

        ~EventTraceWatcher()
        {
            Cleanup();
        }

        public event EventHandler<EventArrivedEventArgs> EventArrived;

        public ulong MatchAnyKeyword { get; set; }

        public TraceLevel Level { get; set; }

        public int BufferSizeInKiloBytes { get; set; }

        private void Cleanup()
        {
            SetEnabled(false);

#if UseTraceEventCache
            foreach (TraceEventInfoWrapper value in this.traceEventInfoCache.Values)
            {
                value.Dispose();
            }
            this.traceEventInfoCache = null;
#endif
        }

        private EventArrivedEventArgs CreateEventArgsFromEventRecord(EventRecord eventRecord)
        {
            byte eventOpcode = eventRecord.EventHeader.EventDescriptor.Opcode;
            TraceEventInfoWrapper traceEventInfo;
            bool shouldDispose = false;

#if UseTraceEventCache
            // Find the event information (schema).
            int index = this.traceEventInfoCache.IndexOfKey(eventOpcode);
            if (index >= 0)
            {
                traceEventInfo = this.traceEventInfoCache.Values[index];
            }
            else
            {
                traceEventInfo = new TraceEventInfoWrapper(eventRecord);
                try
                {
                    this.traceEventInfoCache.Add(eventOpcode, traceEventInfo);
                }
                catch (ArgumentException)
                {
                    // Some other thread added this entry.
                    shouldDispose = true;
                }
            }
#else
            traceEventInfo = new TraceEventInfoWrapper(eventRecord);
            shouldDispose = true;
#endif

            // Get the properties using the current event information (schema).
            PropertyBag properties = traceEventInfo.GetProperties(eventRecord);
            // Dispose the event information because it doesn't live in the cache
            if (shouldDispose)
            {
                traceEventInfo.Dispose();
            }

            EventArrivedEventArgs args = new EventArrivedEventArgs(eventOpcode, properties);
            return args;
        }

        public void Dispose()
        {
            Cleanup();
            GC.SuppressFinalize(this);
        }

        private void EventRecordCallback([In] ref EventRecord eventRecord)
        {
            EventHandler<EventArrivedEventArgs> eventArrived = this.EventArrived;
            if (eventArrived != null)
            {
                EventArrivedEventArgs e = CreateEventArgsFromEventRecord(eventRecord);
                eventArrived(this, e);
            }
        }

        private bool LoadExistingEventTraceProperties()
        {
            const int ERROR_WMI_INSTANCE_NOT_FOUND = 4201;
            this.eventTraceProperties = new EventTraceProperties(true);
            int status = NativeMethods.QueryTrace(0, this.loggerName, ref this.eventTraceProperties);
            if (status == 0)
            {
                return true;
            }
            else if (status == ERROR_WMI_INSTANCE_NOT_FOUND)
            {
                // The instance name passed was not recognized as valid by a WMI data provider.
                return false;
            }
            throw new System.ComponentModel.Win32Exception(status);
        }

        private void ProcessTraceInBackground(object state)
        {
            TraceSafeHandle traceHandle = (TraceSafeHandle)state;
            do
            {
                ulong[] array = { traceHandle.UnsafeValue };

                // Begin receiving the events handled by EventRecordCallback.
                // It is a blocking call until the trace handle gets closed.
                int status = NativeMethods.ProcessTrace(array, 1, IntPtr.Zero, IntPtr.Zero);
                if (!this.closing)
                {
                    // Wait before retry.
                    Thread.Sleep(TimeSpan.FromSeconds(2));

                    Debug.WriteLine(
                        string.Format(CultureInfo.InvariantCulture, "ProcessTrace exited with code {0}.", status));
                }
            } while (!this.closing);
        }

        private void SetEnabled(bool value)
        {
            if (this.enabled == value)
            {
                return;
            }

            if (value)
            {
                StartTracing();
            }
            else
            {
                StopTracing();
            }
            this.enabled = value;
        }

        public void Start()
        {
            SetEnabled(true);
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        private void StartTracing()
        {
            const uint RealTime = 0x00000100;
            const uint EventRecord = 0x10000000;
            uint BufferSize = (uint) this.BufferSizeInKiloBytes;
            const uint MinBuffers = 20;
            const uint MaxBuffers = 200;
            const uint FlushTimerSeconds = 1;
            int status;

            if (!LoadExistingEventTraceProperties())
            {
                this.eventTraceProperties.SetParameters(RealTime, BufferSize, MinBuffers, MaxBuffers, FlushTimerSeconds);

                // Start trace session
                ulong unsafeSessionHandle;
                status = NativeMethods.StartTrace(out unsafeSessionHandle, this.loggerName, ref this.eventTraceProperties);
                if (status != 0)
                {
                    throw new System.ComponentModel.Win32Exception(status);
                }
                this.sessionHandle = new SessionSafeHandle(unsafeSessionHandle, this.loggerName);

                Guid EmptyGuid = Guid.Empty;

                Version Windows7Version = new Version(6, 1, 7600);
                if (Environment.OSVersion.Version.CompareTo(Windows7Version) >= 0)
                {
                    const int TimeToWaitForInitialize = 10 * 1000;
                    EnableTraceParameters enableParameters = new EnableTraceParameters();
                    enableParameters.Version = 1; // ENABLE_TRACE_PARAMETERS_VERSION
                    enableParameters.EnableProperty = EventEnableProperty.Sid;
                    status = NativeMethods.EnableTraceEx2(
                                unsafeSessionHandle,
                                ref this.eventProviderId,
                                1, // controlCode - EVENT_CONTROL_CODE_ENABLE_PROVIDER
                                (byte)this.Level,
                                this.MatchAnyKeyword,
                                0, // matchAnyKeyword
                                TimeToWaitForInitialize,
                                ref enableParameters);
                }
                else
                {
                    status = NativeMethods.EnableTraceEx(
                                ref this.eventProviderId,
                                ref EmptyGuid,          // sourceId
                                unsafeSessionHandle,
                                1,                      // isEnabled
                                (byte)this.Level,
                                this.MatchAnyKeyword,
                                0,                      // matchAllKeywords
                                EventEnableProperty.Sid,
                                IntPtr.Zero);
                }
                if (status != 0)
                {
                    throw new System.ComponentModel.Win32Exception(status);
                }
            }

            this.logFile = new EventTraceLogfile();
            this.logFile.LoggerName = this.loggerName;
            this.logFile.EventRecordCallback = EventRecordCallback;

            this.logFile.ProcessTraceMode = EventRecord | RealTime;
            ulong unsafeTraceHandle = NativeMethods.OpenTrace(ref this.logFile);
            status = Marshal.GetLastWin32Error();
            if (status != 0)
            {
                throw new System.ComponentModel.Win32Exception(status);
            }
            this.traceHandle = new TraceSafeHandle(unsafeTraceHandle);
            this.processEventsThread = new Thread(ProcessTraceInBackground);
            this.closing = false;
            this.processEventsThread.Start(this.traceHandle);
        }

        public void Stop()
        {
            SetEnabled(false);
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        private void StopTracing()
        {
            this.closing = true;

            if (this.traceHandle != null)
            {
                this.traceHandle.Dispose();
                this.traceHandle = null;
            }

            if (this.sessionHandle != null)
            {
                this.sessionHandle.Dispose();
                this.sessionHandle = null;
            }

            if (this.processEventsThread != null)
            {
                this.processEventsThread.Join();
                this.processEventsThread = null;
            }
        }

        private sealed class TraceSafeHandle : SafeHandle
        {
            private ulong traceHandle;

            [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
            public TraceSafeHandle(ulong handle)
                : base(IntPtr.Zero, true)
            {
                this.traceHandle = handle;
            }

            public override bool IsInvalid
            {
                get
                {
                    return this.traceHandle == 0;
                }
            }

            internal ulong UnsafeValue
            {
                get
                {
                    return this.traceHandle;
                }
            }

            protected override bool ReleaseHandle()
            {
                return NativeMethods.CloseTrace(this.traceHandle) != 0;
            }
        }

        private sealed class SessionSafeHandle : SafeHandle
        {
            private readonly ulong sessionHandle;
            private readonly string loggerName;

            [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
            public SessionSafeHandle(ulong sessionHandle, string loggerName)
                : base(IntPtr.Zero, true)
            {
                this.sessionHandle = sessionHandle;
                this.loggerName = loggerName;
            }
            public override bool IsInvalid
            {
                get
                {
                    return this.sessionHandle == 0;
                }
            }

            protected override bool ReleaseHandle()
            {
                EventTraceProperties properties = new EventTraceProperties(true /*initialize*/);
                return NativeMethods.StopTrace(this.sessionHandle, this.loggerName, out properties /*as statistics*/) != 0;
            }
        }
    }
}