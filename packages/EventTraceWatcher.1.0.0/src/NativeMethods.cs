//------------------------------------------------------------------------------
// Author: Daniel Vasquez-Lopez 2009
//------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Samples.Eventing.Interop
{
    [SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        [DllImport("advapi32.dll", ExactSpelling = true, EntryPoint = "StartTraceW", SetLastError = false, CharSet = CharSet.Unicode)]
        internal static extern int StartTrace(
            [Out]
            out ulong sessionHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            string sessionName,
            [In, Out]
            ref EventTraceProperties eventTraceProperties);

        [DllImport("advapi32.dll", ExactSpelling = true, EntryPoint = "QueryTraceW", SetLastError = false, CharSet = CharSet.Unicode)]
        internal static extern int QueryTrace(
            [In]
            ulong sessionHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            string sessionName,
            [In, Out]
            ref EventTraceProperties eventTraceProperties);

        [DllImport("advapi32.dll", SetLastError = false)]
        internal static extern int EnableTraceEx(
            [In] ref Guid providerId,
            [In] ref Guid sourceId,
            [In] ulong traceHandle,
            [In] uint isEnabled,
            [In] byte traceLevel,
            [In] ulong matchAnyKeyword,
            [In] ulong matchAllKeyword,
            [In] EventEnableProperty enableProperty,
            [In] IntPtr enableFilterDescriptor);

        [DllImport("advapi32.dll", SetLastError = false)]
        internal static extern int EnableTraceEx2(
              [In] ulong traceHandle,
              [In] ref Guid providerId,
              [In] uint controlCode,
              [In] byte traceLevel,
              [In] ulong matchAnyKeyword,
              [In] ulong matchAllKeyword,
              [In] uint timeout,
              [In] ref EnableTraceParameters enableParameters);

        [DllImport("advapi32.dll", SetLastError = false)]
        internal static extern int StopTrace(
            [In]
            ulong sessionHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            string sessionName,
            [Out]
            out EventTraceProperties eventTraceProperties);

        [DllImport("advapi32.dll", ExactSpelling = true, EntryPoint = "OpenTraceW", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern ulong /*session handle*/ OpenTrace(
            [In, Out]
            ref EventTraceLogfile logfile);

        [DllImport("advapi32.dll", ExactSpelling = true, EntryPoint = "ProcessTrace", SetLastError = false)]
        internal static extern int ProcessTrace(
            [In]
            ulong[] handleArray,
            [In]
            uint handleCount,
            [In]
            IntPtr startTime,
            [In]
            IntPtr endTime);

        [DllImport("advapi32.dll", ExactSpelling = true, EntryPoint = "CloseTrace", SetLastError = false)]
        internal static extern int CloseTrace(ulong traceHandle);

        [DllImport("tdh.dll", ExactSpelling = true, EntryPoint = "TdhGetEventInformation", SetLastError = false)]
        internal static extern int TdhGetEventInformation(
            [In]
            ref EventRecord Event,
            [In]
            uint TdhContextCount,
            [In]
            IntPtr TdhContext,
            [Out] IntPtr eventInfoPtr,
            [In, Out]
            ref int BufferSize);
    }

    internal delegate void EventRecordCallback([In] ref EventRecord eventRecord);

    [Flags]
    internal enum EventEnableProperty : uint
    {
        None = 0,
        Sid = 1,
        TsId = 2,
        StackTrace = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EnableTraceParameters
    {
        internal uint Version;
        internal EventEnableProperty EnableProperty;
        private uint ControlFlags;
        private Guid SourceId;
        private IntPtr EnableFilterDescriptor;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct Win32TimeZoneInfo
    {
        private int Bias;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        private char[] StandardName;
        private SystemTime StandardDate;
        private int StandardBias;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        private char[] DaylightName;
        private SystemTime DaylightDate;
        private int DaylightBias;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SystemTime
    {
        private short Year;
        private short Month;
        private short DayOfWeek;
        private short Day;
        private short Hour;
        private short Minute;
        private short Second;
        private short Milliseconds;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TraceLogfileHeader
    {
        private uint BufferSize;
        private uint Version;
        private uint ProviderVersion;
        private uint NumberOfProcessors;
        private long EndTime;
        private uint TimerResolution;
        private uint MaximumFileSize;
        private uint LogFileMode;
        private uint BuffersWritten;
        private Guid LogInstanceGuid;
        private IntPtr LoggerName;
        private IntPtr LogFileName;
        private Win32TimeZoneInfo TimeZone;
        private long BootTime;
        private long PerfFreq;
        private long StartTime;
        private uint ReservedFlags;
        private uint BuffersLost;
    }

    [Flags]
    internal enum PropertyFlags
    {
        None = 0,
        Struct = 0x1,
        ParamLength = 0x2,
        ParamCount = 0x4,
        WbemXmlFragment = 0x8,
        ParamFixedLength = 0x10
    }

    internal enum TdhInType : ushort
    {
        Null,
        UnicodeString,
        AnsiString,
        Int8,
        UInt8,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Int64,
        UInt64,
        Float,
        Double,
        Boolean,
        Binary,
        Guid,
        Pointer,
        FileTime,
        SystemTime,
        SID,
        HexInt32,
        HexInt64,  // End of winmeta intypes
        CountedString = 300, // Start of TDH intypes for WBEM
        CountedAnsiString,
        ReversedCountedString,
        ReversedCountedAnsiString,
        NonNullTerminatedString,
        NonNullTerminatedAnsiString,
        UnicodeChar,
        AnsiChar,
        SizeT,
        HexDump,
        WbemSID
    };

    internal enum TdhOutType : ushort
    {
        Null,
        String,
        DateTime,
        Byte,
        UnsignedByte,
        Short,
        UnsignedShort,
        Int,
        UnsignedInt,
        Long,
        UnsignedLong,
        Float,
        Double,
        Boolean,
        Guid,
        HexBinary,
        HexInt8,
        HexInt16,
        HexInt32,
        HexInt64,
        PID,
        TID,
        PORT,
        IPV4,
        IPV6,
        SocketAddress,
        CimDateTime,
        EtwTime,
        Xml,
        ErrorCode,              // End of winmeta outtypes
        ReducedString = 300,    // Start of TDH outtypes for WBEM
        NoPrint
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct WNodeHeader
    {
        [FieldOffset(0)]
        internal int BufferSize;
        [FieldOffset(4)]
        private uint ProviderId;
        [FieldOffset(8)]
        private ulong HistoricalContext;
        [FieldOffset(8)]
        private VersionLinkageType VersionLinkage;
        [FieldOffset(16)]
        private IntPtr KernelHandle;
        [FieldOffset(16)]
        private long TimeStamp;
        [FieldOffset(24)]
        private Guid Guid;
        [FieldOffset(40)]
        private uint ClientContext;
        [FieldOffset(44)]
        internal uint Flags;

        [StructLayout(LayoutKind.Sequential)]
        private struct VersionLinkageType
        {
            private uint Version;
            private uint Linkage;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct EventTraceProperties
    {
        private const int MaxPath = 260;

        private EventTracePropertiesInternal Internal;
        // User-defined fields.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxPath)]
        private char[] LogFileNameBuffer;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxPath)]
        private char[] LoggerNameBuffer;

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        internal EventTraceProperties(bool initialize)
        {
            this.Internal = new EventTracePropertiesInternal();
            this.LogFileNameBuffer = null;
            this.LoggerNameBuffer = null;

            if (initialize)
            {
                const uint WNODE_FLAG_TRACED_GUID = 0x00020000;

                this.Internal.Wnode.BufferSize = Marshal.SizeOf(typeof(EventTracePropertiesInternal)) + MaxPath * 2 /*unicode*/ * 2 /*fields*/;
                System.Diagnostics.Debug.Assert(this.Internal.Wnode.BufferSize == 1160);
                this.Internal.Wnode.Flags = WNODE_FLAG_TRACED_GUID;

                this.Internal.LogFileNameOffset = Marshal.SizeOf(typeof(EventTracePropertiesInternal));
                System.Diagnostics.Debug.Assert(this.Internal.LogFileNameOffset == 120);
                this.Internal.LoggerNameOffset = this.Internal.LogFileNameOffset + MaxPath * 2 /*unicode*/;
            }
        }

        internal void SetParameters(uint logFileMode, uint bufferSize, uint minBuffers, uint maxBuffers, uint flushTimerSeconds)
        {
            this.Internal.LogFileMode = logFileMode;

            // Set the buffer size. BufferSize is in KB.
            this.Internal.BufferSize = bufferSize;
            this.Internal.MinimumBuffers = minBuffers;
            this.Internal.MaximumBuffers = maxBuffers;

            // Number of seconds before timer is flushed.
            this.Internal.FlushTimer = flushTimerSeconds;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EventTracePropertiesInternal
        {
            internal WNodeHeader Wnode;
            internal uint BufferSize;
            internal uint MinimumBuffers;
            internal uint MaximumBuffers;
            private uint MaximumFileSize;
            internal uint LogFileMode;
            internal uint FlushTimer;
            private uint EnableFlags;
            private int AgeLimit;
            private uint NumberOfBuffers;
            private uint FreeBuffers;
            private uint EventsLost;
            private uint BuffersWritten;
            private uint LogBuffersLost;
            private uint RealTimeBuffersLost;
            private IntPtr LoggerThreadId;
            internal int LogFileNameOffset;
            internal int LoggerNameOffset;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct EventPropertyInfo
    {
        [FieldOffset(0)]
        private PropertyFlags Flags;
        [FieldOffset(4)]
        internal uint NameOffset;
        [FieldOffset(8)]
        internal NonStructType NonStructTypeValue;
        [FieldOffset(8)]
        private StructType StructTypeValue;
        [FieldOffset(16)]
        private ushort CountPropertyIndex;
        [FieldOffset(18)]
        internal ushort LengthPropertyIndex;
        [FieldOffset(20)]
        private uint _Reserved;

        [StructLayout(LayoutKind.Sequential)]
        internal struct NonStructType
        {
            internal TdhInType InType;
            private TdhOutType OutType;
            internal uint MapNameOffset;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct StructType
        {
            private ushort StructStartIndex;
            private ushort NumOfStructMembers;
            private uint _Padding;
        }
    }

    internal enum TemplateFlags
    {
        TemplateEventDdata = 1,
        TemplateUserData = 2
    }

    internal enum DecodingSource
    {
        DecodingSourceXmlFile,
        DecodingSourceWbem,
        DecodingSourceWPP
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TraceEventInfo
    {
        private Guid ProviderGuid;
        private Guid EventGuid;
        private EtwEventDescriptor EventDescriptor;
        private DecodingSource DecodingSource;
        private uint ProviderNameOffset;
        private uint LevelNameOffset;
        private uint ChannelNameOffset;
        private uint KeywordsNameOffset;
        private uint TaskNameOffset;
        internal uint OpcodeNameOffset;
        private uint EventMessageOffset;
        private uint ProviderMessageOffset;
        private uint BinaryXmlOffset;
        private uint BinaryXmlSize;
        private uint ActivityIDNameOffset;
        private uint RelatedActivityIDNameOffset;
        private uint PropertyCount;
        internal int TopLevelPropertyCount;
        private TemplateFlags Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EventTraceHeader
    {
        private ushort Size;
        private ushort FieldTypeFlags;
        private uint Version;
        private uint ThreadId;
        private uint ProcessId;
        private long TimeStamp;
        private Guid Guid;
        private uint KernelTime;
        private uint UserTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EventTrace
    {
        private EventTraceHeader Header;
        private uint InstanceId;
        private uint ParentInstanceId;
        private Guid ParentGuid;
        private IntPtr MofData;
        private uint MofLength;
        private uint ClientContext;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct EventTraceLogfile
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        private string LogFileName;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string LoggerName;
        private Int64 CurrentTime;
        private uint BuffersRead;
        internal uint ProcessTraceMode;
        private EventTrace CurrentEvent;
        private TraceLogfileHeader LogfileHeader;
        private IntPtr BufferCallback;
        private uint BufferSize;
        private uint Filled;
        private uint EventsLost;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        internal EventRecordCallback EventRecordCallback;
        private uint IsKernelTrace;
        private IntPtr Context;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EtwEventDescriptor
    {
        private ushort Id;
        private byte Version;
        private byte Channel;
        private byte Level;
        internal byte Opcode;
        private ushort Task;
        private ulong Keyword;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EventHeader
    {
        private ushort Size;
        private ushort HeaderType;
        private ushort Flags;
        private ushort EventProperty;
        private uint ThreadId;
        private uint ProcessId;
        internal Int64 TimeStamp;
        private Guid ProviderId;
        internal EtwEventDescriptor EventDescriptor;
        private ulong ProcessorTime;
        private Guid ActivityId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EventRecord
    {
        internal EventHeader EventHeader;
        private EtwBufferContext BufferContext;
        private ushort ExtendedDataCount;
        internal ushort UserDataLength;
        private IntPtr ExtendedData;
        internal IntPtr UserData;
        internal IntPtr UserContext;

        [StructLayout(LayoutKind.Sequential)]
        private struct EtwBufferContext
        {
            private byte ProcessorNumber;
            private byte Alignment;
            private ushort LoggerId;
        }
    }
}