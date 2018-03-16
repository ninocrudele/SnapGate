// Copyright 2016-2040 Nino Crudele
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#region Usings

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

#endregion

namespace Core.Eventing.Interop
{
    [SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        [DllImport("advapi32.dll", ExactSpelling = true, EntryPoint = "StartTraceW", SetLastError = false,
            CharSet = CharSet.Unicode)]
        internal static extern int StartTrace(
            [Out] out ulong sessionHandle,
            [In] [MarshalAs(UnmanagedType.LPWStr)] string sessionName,
            [In] [Out] ref EventTraceProperties eventTraceProperties);

        [DllImport("advapi32.dll", ExactSpelling = true, EntryPoint = "QueryTraceW", SetLastError = false,
            CharSet = CharSet.Unicode)]
        internal static extern int QueryTrace(
            [In] ulong sessionHandle,
            [In] [MarshalAs(UnmanagedType.LPWStr)] string sessionName,
            [In] [Out] ref EventTraceProperties eventTraceProperties);

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
            [In] ulong sessionHandle,
            [In] [MarshalAs(UnmanagedType.LPWStr)] string sessionName,
            [Out] out EventTraceProperties eventTraceProperties);

        [DllImport("advapi32.dll", ExactSpelling = true, EntryPoint = "OpenTraceW", SetLastError = true,
            CharSet = CharSet.Unicode)]
        internal static extern ulong /*session handle*/ OpenTrace([In] [Out] ref EventTraceLogfile logfile);

        [DllImport("advapi32.dll", ExactSpelling = true, EntryPoint = "ProcessTrace", SetLastError = false)]
        internal static extern int ProcessTrace(
            [In] ulong[] handleArray,
            [In] uint handleCount,
            [In] IntPtr startTime,
            [In] IntPtr endTime);

        [DllImport("advapi32.dll", ExactSpelling = true, EntryPoint = "CloseTrace", SetLastError = false)]
        internal static extern int CloseTrace(ulong traceHandle);

        [DllImport("tdh.dll", ExactSpelling = true, EntryPoint = "TdhGetEventInformation", SetLastError = false)]
        internal static extern int TdhGetEventInformation(
            [In] ref EventRecord Event,
            [In] uint TdhContextCount,
            [In] IntPtr tdhContext,
            [Out] IntPtr eventInfoPtr,
            [In] [Out] ref int BufferSize);
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

        private readonly uint ControlFlags;

        internal Guid SourceId;

        internal IntPtr EnableFilterDescriptor;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct Win32TimeZoneInfo
    {
        internal int Bias;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        internal char[] StandardName;

        internal SystemTime StandardDate;

        internal int StandardBias;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        internal char[] DaylightName;

        internal SystemTime DaylightDate;

        internal int DaylightBias;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SystemTime
    {
        internal short Year;

        internal short Month;

        internal short DayOfWeek;

        internal short Day;

        internal short Hour;

        internal short Minute;

        internal short Second;

        internal short Milliseconds;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TraceLogfileHeader
    {
        internal uint BufferSize;

        internal uint Version;

        internal uint ProviderVersion;

        internal uint NumberOfProcessors;

        internal long EndTime;

        internal uint TimerResolution;

        internal uint MaximumFileSize;

        internal uint LogFileMode;

        internal uint BuffersWritten;

        internal Guid LogInstanceGuid;

        internal IntPtr LoggerName;

        internal IntPtr LogFileName;

        internal Win32TimeZoneInfo TimeZone;

        internal long BootTime;

        internal long PerfFreq;

        internal long StartTime;

        internal uint ReservedFlags;

        internal uint BuffersLost;
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

        HexInt64, // End of winmeta intypes

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
    }

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

        ErrorCode, // End of winmeta outtypes

        ReducedString = 300, // Start of TDH outtypes for WBEM

        NoPrint
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct WNodeHeader
    {
        [FieldOffset(0)] internal int BufferSize;

        [FieldOffset(4)] internal uint ProviderId;

        [StructLayout(LayoutKind.Sequential)]
        internal struct VersionLinkageType
        {
            internal uint Version;

            internal uint Linkage;
        }

        [FieldOffset(8)] internal ulong HistoricalContext;

        [FieldOffset(8)] internal VersionLinkageType VersionLinkage;

        [FieldOffset(16)] internal IntPtr KernelHandle;

        [FieldOffset(16)] internal long TimeStamp;

        [FieldOffset(24)] internal Guid Guid;

        [FieldOffset(40)] internal uint ClientContext;

        [FieldOffset(44)] internal uint Flags;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct EventTraceProperties
    {
        private const int MaxPath = 260;

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        internal EventTraceProperties(bool initialize)
        {
            Internal = new EventTracePropertiesInternal();
            LogFileNameBuffer = null;
            LoggerNameBuffer = null;

            if (initialize)
            {
                const uint WNODE_FLAG_TRACED_GUID = 0x00020000;

                Internal.Wnode.BufferSize = Marshal.SizeOf(typeof(EventTracePropertiesInternal))
                                            + MaxPath * 2 /*unicode*/ * 2 /*fields*/;
                Debug.Assert(Internal.Wnode.BufferSize == 1160);
                Internal.Wnode.Flags = WNODE_FLAG_TRACED_GUID;

                Internal.LogFileNameOffset = Marshal.SizeOf(typeof(EventTracePropertiesInternal));
                Debug.Assert(Internal.LogFileNameOffset == 120);
                Internal.LoggerNameOffset = Internal.LogFileNameOffset + MaxPath * 2 /*unicode*/;
            }
        }

        internal void SetParameters(
            uint logFileMode,
            uint bufferSize,
            uint minBuffers,
            uint maxBuffers,
            uint flushTimerSeconds)
        {
            Internal.LogFileMode = logFileMode;

            // Set the buffer size. BufferSize is in KB.
            Internal.BufferSize = bufferSize;
            Internal.MinimumBuffers = minBuffers;
            Internal.MaximumBuffers = maxBuffers;

            // Number of seconds before timer is flushed.
            Internal.FlushTimer = flushTimerSeconds;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EventTracePropertiesInternal
        {
            internal WNodeHeader Wnode;

            internal uint BufferSize;

            internal uint MinimumBuffers;

            internal uint MaximumBuffers;

            internal readonly uint MaximumFileSize;

            internal uint LogFileMode;

            internal uint FlushTimer;

            internal readonly uint EnableFlags;

            internal readonly int AgeLimit;

            internal readonly uint NumberOfBuffers;

            internal readonly uint FreeBuffers;

            internal readonly uint EventsLost;

            internal readonly uint BuffersWritten;

            internal readonly uint LogBuffersLost;

            internal readonly uint RealTimeBuffersLost;

            internal readonly IntPtr LoggerThreadId;

            internal int LogFileNameOffset;

            internal int LoggerNameOffset;
        }

        private EventTracePropertiesInternal Internal;

        // User-defined fields.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxPath)]
        private readonly char[] LogFileNameBuffer;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxPath)]
        private readonly char[] LoggerNameBuffer;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct EventPropertyInfo
    {
        [FieldOffset(0)] internal PropertyFlags Flags;

        [FieldOffset(4)] internal uint NameOffset;

        [StructLayout(LayoutKind.Sequential)]
        internal struct NonStructType
        {
            internal TdhInType InType;

            internal TdhOutType OutType;

            internal uint MapNameOffset;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct StructType
        {
            internal ushort StructStartIndex;

            internal ushort NumOfStructMembers;

            private readonly uint _Padding;
        }

        [FieldOffset(8)] internal NonStructType NonStructTypeValue;

        [FieldOffset(8)] internal StructType StructTypeValue;

        [FieldOffset(16)] internal ushort CountPropertyIndex;

        [FieldOffset(18)] internal ushort LengthPropertyIndex;

        [FieldOffset(20)] private readonly uint _Reserved;
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
        internal Guid ProviderGuid;

        internal Guid EventGuid;

        internal EtwEventDescriptor EventDescriptor;

        internal DecodingSource DecodingSource;

        internal uint ProviderNameOffset;

        internal uint LevelNameOffset;

        internal uint ChannelNameOffset;

        internal uint KeywordsNameOffset;

        internal uint TaskNameOffset;

        internal uint OpcodeNameOffset;

        internal uint EventMessageOffset;

        internal uint ProviderMessageOffset;

        internal uint BinaryXmlOffset;

        internal uint BinaryXmlSize;

        internal uint ActivityIDNameOffset;

        internal uint RelatedActivityIDNameOffset;

        internal uint PropertyCount;

        internal int TopLevelPropertyCount;

        internal TemplateFlags Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EventTraceHeader
    {
        internal ushort Size;

        internal ushort FieldTypeFlags;

        internal uint Version;

        internal uint ThreadId;

        internal uint ProcessId;

        internal long TimeStamp;

        internal Guid Guid;

        internal uint KernelTime;

        internal uint UserTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EventTrace
    {
        internal EventTraceHeader Header;

        internal uint InstanceId;

        internal uint ParentInstanceId;

        internal Guid ParentGuid;

        internal IntPtr MofData;

        internal uint MofLength;

        internal uint ClientContext;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct EventTraceLogfile
    {
        [MarshalAs(UnmanagedType.LPWStr)] internal string LogFileName;

        [MarshalAs(UnmanagedType.LPWStr)] internal string LoggerName;

        internal long CurrentTime;

        internal uint BuffersRead;

        internal uint ProcessTraceMode;

        internal EventTrace CurrentEvent;

        internal TraceLogfileHeader LogfileHeader;

        internal IntPtr BufferCallback;

        internal uint BufferSize;

        internal uint Filled;

        internal uint EventsLost;

        [MarshalAs(UnmanagedType.FunctionPtr)] internal EventRecordCallback EventRecordCallback;

        internal uint IsKernelTrace;

        internal IntPtr Context;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EtwEventDescriptor
    {
        internal ushort Id;

        internal byte Version;

        internal byte Channel;

        internal byte Level;

        internal byte Opcode;

        internal ushort Task;

        internal ulong Keyword;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EventHeader
    {
        internal ushort Size;

        internal ushort HeaderType;

        internal ushort Flags;

        internal ushort EventProperty;

        internal uint ThreadId;

        internal uint ProcessId;

        internal long TimeStamp;

        internal Guid ProviderId;

        internal EtwEventDescriptor EventDescriptor;

        internal ulong ProcessorTime;

        internal Guid ActivityId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EventRecord
    {
        internal EventHeader EventHeader;

        internal EtwBufferContext BufferContext;

        internal ushort ExtendedDataCount;

        internal ushort UserDataLength;

        internal IntPtr ExtendedData;

        internal IntPtr UserData;

        internal IntPtr UserContext;

        [StructLayout(LayoutKind.Sequential)]
        internal struct EtwBufferContext
        {
            internal byte ProcessorNumber;

            internal byte Alignment;

            internal ushort LoggerId;
        }
    }
}