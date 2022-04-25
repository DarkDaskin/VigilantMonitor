using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace VigilantMonitor
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class NativeMethods
    {
        [DllImport("kernel32", SetLastError = true)]
        public static extern SafePowerRequestHandle PowerCreateRequest(in REASON_CONTEXT context);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool PowerSetRequest(SafePowerRequestHandle powerRequest, POWER_REQUEST_TYPE requestType);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool PowerClearRequest(SafePowerRequestHandle powerRequest, POWER_REQUEST_TYPE requestType);

        [DllImport("kernel32")]
        public static extern bool CloseHandle(IntPtr hHandle);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct REASON_CONTEXT
        {
            public const uint POWER_REQUEST_CONTEXT_VERSION = 0;
            public const uint POWER_REQUEST_CONTEXT_SIMPLE_STRING = 1;

            public uint Version;
            public uint Flags;
            public string SimpleReasonString;
        }

        public enum POWER_REQUEST_TYPE
        {
            PowerRequestDisplayRequired,
            PowerRequestSystemRequired,
            PowerRequestAwayModeRequired,
            PowerRequestExecutionRequired
        }

        public class SafePowerRequestHandle : SafeHandleMinusOneIsInvalid
        {
            public SafePowerRequestHandle() : base(true) { }

            protected override bool ReleaseHandle() => CloseHandle(handle);
        }
    }
}