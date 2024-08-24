﻿using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.Interop.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
        private bool Patched;

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
            OriginalBytes = MemoryAccessor.MemRead(Address, BytesToPatch.Length);
        }

        public void Patch()
        {
            MemoryAccessor.MemWrite(Address, BytesToPatch);
            Patched = true;
        }

        public void UnPatch()
        {
            MemoryAccessor.MemWrite(Address, OriginalBytes);
            Patched = false;
        }

        public bool IsPatched() => Patched;

        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        public static void InitializeMemoryPatches(object self)
        {
            Type selfType = self.GetType();
            var fields = selfType.GetFields(Flags).Select(field => (field, field.GetCustomAttribute<MemPatchAttribute>())).Where(tuple => tuple.Item2 != null);
            foreach (var (field, attribute) in fields)
            {
                string fieldName = field.Name;

                string pattern = attribute!.Pattern;
                string bytesToPatch = attribute!.BytesToPatch;
                CModule module = attribute!.Module;

                try
                {
                    object? instance = Activator.CreateInstance(typeof(CMemPatch), pattern, bytesToPatch, module);
                    field.SetValue(self, instance);
                    instance?.GetType().GetMethod("Patch")?.Invoke(instance, null);
                    Application.Instance.Logger.LogInformation($"Initialized MemPatch field {fieldName} and Patched.");
                }
                catch (Exception ex)
                {
                    Application.Instance.Logger.LogInformation($"Failed to initialize MemPatch field {fieldName}.\n{ex.StackTrace}");
                }
            }
        }
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

        private const uint PAGE_EXECUTE_READWRITE = 0x40;

        private static readonly nint ProcessHandle = Process.GetCurrentProcess().Handle;

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

        internal static byte[] MemRead(nint address, int size)
        {
            byte[] buf = new byte[size];
            if (IsWindows)
            {
                if (!ReadProcessMemory(ProcessHandle, address, buf, (uint)size, out _))
                    throw new Exception("Failed to read memory.");
            }
            else
            {
                nint _handle = open("/proc/self/mem", O_RDONLY);

                if (_handle == 0)
                    throw new Exception($"Failed to open /proc/self/mem.");

                long result = pread(_handle, buf, (ulong)size, (ulong)address);
                if (result == -1)
                    throw new Exception("Failed to read memory.");

                close(_handle);
            }

            return buf;
        }

        internal static void MemWrite(nint address, byte[] buf)
        {
            if (IsWindows)
            {
                uint oldProtect;

                if (!VirtualProtectEx(ProcessHandle, address, (nuint)buf.Length, PAGE_EXECUTE_READWRITE, out oldProtect))
                    throw new Exception("Failed to change memory protection.");

                if (!WriteProcessMemory(ProcessHandle, address, buf, (uint)buf.Length, out _))
                    throw new Exception("Failed to write memory.");

                if (!VirtualProtectEx(ProcessHandle, address, (nuint)buf.Length, oldProtect, out _))
                    throw new Exception("Failed to restore memory protection.");
            }
            else
            {
                nint _handle = open("/proc/self/mem", O_RDWR);

                if (_handle == 0)
                    throw new Exception($"Failed to open /proc/self/mem.");

                long result = pwrite(_handle, buf, (ulong)buf.Length, (ulong)address);
                if (result == -1)
                    throw new Exception("Failed to write memory.");

                close(_handle);
            }
        }
    }
}
