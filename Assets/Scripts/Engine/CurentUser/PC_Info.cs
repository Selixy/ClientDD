// PC_Info.cs
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace RPG_System.User
{
    public static class PC_Info
    {
        public static float CPUUsagePercent { get; private set; }
        public static float SystemCPUUsagePercent { get; private set; }
        public static float RAMUsedMB { get; private set; }
        public static float RAMTotalMB { get; private set; }
        public static float RAMAvailableMB => RAMTotalMB - RAMUsedMB;

        public static float CPUAvailablePercent => 100f - CPUUsagePercent;
        public static float SystemCPUAvailablePercent => 100f - SystemCPUUsagePercent;

        public static float ComputationScore
        {
            get
            {
                float loadFactor  = 1f - (CPUUsagePercent       / 100f);
                float availFactor =  SystemCPUAvailablePercent / 100f;
                float score       = (loadFactor + availFactor) * 50f;
                if (score < 0f)   return 0f;
                if (score > 100f) return 100f;
                return score;
            }
        }

        private static readonly Process _process;
        private static TimeSpan        _prevProcTime;
        private static DateTime        _prevSampleTime;
        private static ulong           _prevIdle, _prevKernel, _prevUser;
        private static readonly int    _coreCount;
        private static readonly Timer  _timer;

        static PC_Info()
        {
            _process        = Process.GetCurrentProcess();
            _prevProcTime   = _process.TotalProcessorTime;
            _prevSampleTime = DateTime.UtcNow;
            _coreCount      = Environment.ProcessorCount;
            RAMTotalMB      = GetTotalMemoryMB();
            TryGetSystemTimes(out _prevIdle, out _prevKernel, out _prevUser);
            _timer = new Timer(_ => Sample(), null, 0, 1000);
        }

        public static void Stop() => _timer.Dispose();

        private static void Sample()
        {
            var now       = DateTime.UtcNow;
            var elapsedMs = (now - _prevSampleTime).TotalMilliseconds;
            if (elapsedMs <= 0) elapsedMs = 1;

            _process.Refresh();
            var currentProc  = _process.TotalProcessorTime;
            var procDeltaMs  = (currentProc - _prevProcTime).TotalMilliseconds;
            CPUUsagePercent  = (float)(procDeltaMs / (elapsedMs * _coreCount) * 100.0);
            _prevProcTime    = currentProc;

            if (TryGetSystemTimes(out var idle, out var kernel, out var user))
            {
                var idleDelta  = idle   - _prevIdle;
                var kernDelta  = kernel - _prevKernel;
                var userDelta  = user   - _prevUser;
                var totalDelta = kernDelta + userDelta;
                if (totalDelta > 0)
                    SystemCPUUsagePercent = (float)((totalDelta - idleDelta) * 100.0 / totalDelta);
                _prevIdle   = idle;
                _prevKernel = kernel;
                _prevUser   = user;
            }

            _process.Refresh();
            RAMUsedMB  = _process.WorkingSet64 / (1024f * 1024f);
            RAMTotalMB = GetTotalMemoryMB();
            _prevSampleTime = now;
        }

        private static float GetTotalMemoryMB()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var ms = new MEMORYSTATUSEX();
                if (GlobalMemoryStatusEx(ms))
                    return ms.ullTotalPhys / (1024f * 1024f);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    foreach (var line in File.ReadAllLines("/proc/meminfo"))
                    {
                        if (line.StartsWith("MemTotal:"))
                        {
                            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (ulong.TryParse(parts[1], out var kb))
                                return kb / 1024f;
                        }
                    }
                }
                catch { }
            }
            return 0f;
        }

        private static bool TryGetSystemTimes(out ulong idle, out ulong kernel, out ulong user)
        {
            idle = kernel = user = 0;
            if (!GetSystemTimesNative(out var idleFT, out var kernFT, out var userFT))
                return false;
            idle   = ((ulong)idleFT.dwHighDateTime   << 32) | idleFT.dwLowDateTime;
            kernel = ((ulong)kernFT.dwHighDateTime   << 32) | kernFT.dwLowDateTime;
            user   = ((ulong)userFT.dwHighDateTime   << 32) | userFT.dwLowDateTime;
            return true;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetSystemTimesNative(
            out FILETIME idleTime,
            out FILETIME kernelTime,
            out FILETIME userTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint  dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            public uint  dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }
    }
}
