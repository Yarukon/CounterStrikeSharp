using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Memory.Interop
{
    public class CMemPatch
    {
        private byte[] OriginalBytes;
        private byte[] BytesToPatch;
        private string Pattern;
        private nint Address;

        [ThreadStatic]
        private static nint ProcessHandle = Process.GetCurrentProcess().Handle;

        public CMemPatch(string signature, string bytesToPatch, CModule module = CModule.SERVER)
        {
            Pattern = signature;
            Address = PatternManager.Instance.FindPattern(signature, module);

            if (Address == 0)
                throw new ArgumentException($"Couldn't find signature {signature} in module {module}.");

            // Handle the bytes to patch
            bytesToPatch = bytesToPatch.Trim();
            string[] _bytesToPatch = bytesToPatch.Split(" ");
            BytesToPatch = new byte[_bytesToPatch.Length];

            int incremental = 0;
            foreach (string _byte in _bytesToPatch)
            {
                BytesToPatch[incremental] = Convert.ToByte(_byte, 16);
                incremental++;
            }

            // Copy the original bytes
            OriginalBytes = MemoryAccessor.MemRead(ProcessHandle, Address, BytesToPatch.Length);
        }

        public void Patch() => MemoryAccessor.MemWrite(ProcessHandle, Address, BytesToPatch);
        public void UnPatch() => MemoryAccessor.MemWrite(ProcessHandle, Address, OriginalBytes);
    }

    internal class MemoryAccessor
    {
        private static bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        // Windows
        [DllImport("kernel32.dll")]
        private static extern nint OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, uint nSize, out uint lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, uint nSize, out uint lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtectEx(nint hProcess, nint lpAddress, nuint dwSize, uint flNewProtect, out uint lpflOldProtect);

        private const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
        private const uint PAGE_EXECUTE_READWRITE = 0x40;
        private const uint PAGE_READONLY = 0x02;

        // Linux
        [DllImport("libc.so.6", SetLastError = true)]
        private static extern nint open(string pathname, int flags);

        [DllImport("libc.so.6", SetLastError = true)]
        private static extern int close(nint fd);

        [DllImport("libc.so.6", SetLastError = true)]
        private static extern long pread(nint fd, byte[] buf, ulong count, ulong offset);

        [DllImport("libc.so.6", SetLastError = true)]
        private static extern long pwrite(nint fd, byte[] buf, ulong count, ulong offset);

        private const int O_RDONLY = 0;
        private const int O_RDWR = 0x02;

        internal static byte[] MemRead(nint handle, nint address, int size)
        {
            byte[] buf = new byte[size];
            if (IsWindows)
            {
                uint bytesRead;
                if (!ReadProcessMemory(handle, address, buf, (uint)size, out bytesRead))
                    throw new Exception("Failed to read memory.");
            }
            else
            {
                string memPath = $"/proc/self/mem";
                nint _handle = open(memPath, O_RDONLY);

                if (_handle == 0)
                    throw new Exception($"Failed to open {memPath}.");

                long result = pread(_handle, buf, (ulong)size, (ulong)address);
                if (result == -1)
                    throw new Exception("Failed to read memory.");

                close(_handle);
            }

            return buf;
        }

        internal static void MemWrite(nint handle, nint address, byte[] buf)
        {
            if (IsWindows)
            {
                uint oldProtect;

                if (!VirtualProtectEx(handle, address, (nuint)buf.Length, PAGE_EXECUTE_READWRITE, out oldProtect))
                    throw new Exception("Failed to change memory protection.");

                uint bytesWritten;
                if (!WriteProcessMemory(handle, address, buf, (uint)buf.Length, out bytesWritten))
                    throw new Exception("Failed to write memory.");

                if (!VirtualProtectEx(handle, address, (nuint)buf.Length, oldProtect, out oldProtect))
                    throw new Exception("Failed to restore memory protection.");
            }
            else
            {
                string memPath = $"/proc/self/mem";
                nint _handle = open(memPath, O_RDWR);

                if (_handle == 0)
                    throw new Exception($"Failed to open {memPath}.");

                long result = pwrite(_handle, buf, (ulong)buf.Length, (ulong)address);
                if (result == -1)
                    throw new Exception("Failed to write memory.");

                close(_handle);
            }
        }
    }
}
