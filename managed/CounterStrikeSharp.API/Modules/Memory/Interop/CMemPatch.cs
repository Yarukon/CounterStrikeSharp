using CounterStrikeSharp.API.Core;
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
        public nint GetAddress() => Address;

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
                    Application.Instance.Logger.LogInformation($"Initialized MemPatch field {fieldName} at {(instance as CMemPatch)!.GetAddress():X} and Patched.");
                }
                catch (TargetInvocationException ex)
                {
                    Application.Instance.Logger.LogInformation($"Failed to initialize MemPatch field {fieldName}. {ex.InnerException?.Message}\n{ex.InnerException?.StackTrace}");
                }
                catch (Exception ex)
                {
                    Application.Instance.Logger.LogInformation($"Failed to initialize MemPatch field {fieldName}. {ex.Message}\n{ex.StackTrace}");
                }
            }
        }
    }

    internal static partial class NativeMethods
    {
        // Windows
        [LibraryImport("kernel32.dll", EntryPoint = "VirtualProtect", SetLastError = true)]
        private static partial int _VirtualProtect(nint lpAddress, nuint dwSize, uint flNewProtect, out uint lpflOldProtect);
        internal static bool VirtualProtect(nint lpAddress, nuint dwSize, uint flNewProtect, out uint lpflOldProtect) => _VirtualProtect(lpAddress, dwSize, flNewProtect, out lpflOldProtect) != 0;

        // Linux
        [LibraryImport("libc.so.6", SetLastError = true)]
        internal static partial int mprotect(nint addr, nuint len, int prot);

        [LibraryImport("libc.so.6", SetLastError = true)]
        internal static partial IntPtr strerror(int errnum);

    }

    internal class MemoryAccessor
    {
        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        // Windows
        internal const uint PAGE_EXECUTE_READWRITE = 0x40;

        // Linux
        internal const int PROT_READ = 0x1;
        internal const int PROT_WRITE = 0x2;
        internal const int PROT_EXEC = 0x4;

        private static string? GetLastErrorString()
        {
            if (IsWindows)
                return Marshal.GetLastWin32Error().ToString();
            else
                return Marshal.PtrToStringAnsi(NativeMethods.strerror(Marshal.GetLastPInvokeError()));
        }

        internal static byte[] MemRead(nint address, int size)
        {
            byte[] buffer = new byte[size];
            Marshal.Copy(address, buffer, 0, size);
            return buffer;
        }

        internal static void MemWrite(nint address, byte[] buffer)
        {
            if (IsWindows)
            {
                if (!NativeMethods.VirtualProtect(address, (nuint)buffer.Length, PAGE_EXECUTE_READWRITE, out uint oldProtect))
                    throw new Exception($"Failed to change memory protection at address {address:X}. Error: {GetLastErrorString()}");

                try
                {
                    Marshal.Copy(buffer, 0, address, buffer.Length);
                }
                finally
                {
                    if (!NativeMethods.VirtualProtect(address, (nuint)buffer.Length, oldProtect, out _))
                        throw new Exception($"Failed to restore memory protection at address {address:X}. Error: {GetLastErrorString() ?? "unknown error"}");
                }
            }
            else
            {
                if (NativeMethods.mprotect(address, (nuint)buffer.Length, PROT_READ | PROT_WRITE | PROT_EXEC) != 0)
                    throw new Exception($"Failed to change memory protection at address {address:X}. Error: {GetLastErrorString() ?? "unknown error"}");

                try
                {
                    Marshal.Copy(buffer, 0, address, buffer.Length);
                }
                finally
                {
                    if (NativeMethods.mprotect(address, (nuint)buffer.Length, PROT_READ | PROT_EXEC) != 0)
                        throw new Exception($"Failed to restore memory protection at address {address:X}. Error: {GetLastErrorString() ?? "unknown error"}");
                }
            }
        }
    }
}
