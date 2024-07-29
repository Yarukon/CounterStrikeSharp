using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.Interop.Attributes;
using Iced.Intel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace CounterStrikeSharp.API.Modules.Memory.Interop
{
    public class PatternManager
    {
#pragma warning disable 8618
        public static PatternManager Instance { get; private set; }
#pragma warning restore

        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private ILogger Logger;

        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        internal PatternManager()
        {
            if (Instance != null)
                throw new Exception("Already initialized PatternManager.");

            Instance = this;
            Logger = Application.Instance.Logger;

            Logger.LogInformation("Initialized PatternManager.");
        }

        public nint FindPattern(string pattern, CModule module = CModule.SERVER)
        {
            return module switch
            {
                CModule.SERVER => NativeAPI.FindSignature(IsWindows ? "server.dll" : "libserver.so", pattern),
                CModule.ENGINE => NativeAPI.FindSignature(IsWindows ? "engine2.dll" : "libengine2.so", pattern),
                CModule.NETWORKSYSTEM => NativeAPI.FindSignature(IsWindows ? "networksystem.dll" : "libnetworksystem.so", pattern),
                _ => throw new NotImplementedException($"Invalid module {module}.")
            };
        }

        public nint GetStaticAddressFromPattern(string pattern, int offset = 0, CModule module = CModule.SERVER)
        {
            var instructionAddress = FindPattern(pattern, module);

            if (instructionAddress == 0)
                throw new Exception($"Can't locate the address from the given signature {pattern}.");

            instructionAddress += offset;

            try
            {
                var reader = new UnsafeCodeReader(instructionAddress, pattern.Length + 8);
                var decoder = Decoder.Create(64, reader, (ulong)instructionAddress, DecoderOptions.AMD);
                while (reader.CanReadByte)
                {
                    var instruction = decoder.Decode();
                    if (instruction.IsInvalid) continue;
                    if (instruction.Op0Kind is OpKind.Memory || instruction.Op1Kind is OpKind.Memory)
                        return (nint)instruction.MemoryDisplacement64;
                }
            }
            catch // ignored
            {
            }

            throw new Exception($"Can't find any referenced address in the given signature {pattern}.");
        }

#pragma warning disable 8602, 8600 // Visual Studio真是有够烦的
        public void InitializeMemoryFunctions(object self, string configName = "")
        {
            Dictionary<string, PlatformData> gameDatas = [];

            Type selfType = self.GetType();
            var fields = selfType.GetFields(Flags).Select(field => (field, field.GetCustomAttribute<MemFuncAttribute>())).Where(tuple => tuple.Item2 != null);
            foreach (var (field, attribute) in fields)
            {
                if (!string.IsNullOrWhiteSpace(configName))
                    gameDatas = InteropGameData.ReadFrom(configName);

                string funcName = field.Name;

                string pattern = attribute!.Pattern;
                if (!string.IsNullOrWhiteSpace(configName))
                {
                    if (gameDatas.TryGetValue(pattern, out PlatformData value))
                        pattern = IsWindows ? (string) value.Windows : (string) value.Linux;
                    else
                    {
                        Logger.LogError($"Function {field.Name} defined config name but couldn't find key {pattern}.");
                        continue;
                    }
                }

                nint address = FindPattern(attribute!.Pattern, attribute.Module);
                if (address != 0)
                {
                    Logger.LogInformation($"Found function {funcName} -> 0x{address:X}");
                    Delegate @delegate = Marshal.GetDelegateForFunctionPointer(address, field.FieldType);
                    field.SetValue(self, @delegate);
                }
                else
                    Logger.LogError($"Can't locate the address for {funcName}");
            }
        }
    }
#pragma warning restore

    internal unsafe class UnsafeCodeReader : CodeReader
    {
        private readonly int length;
        private readonly byte* address;
        private int pos;

        public UnsafeCodeReader(nint address, int length)
        {
            this.length = length;
            this.address = (byte*) address;
        }

        public bool CanReadByte => this.pos < this.length;

        public override int ReadByte()
        {
            if (this.pos >= this.length) return -1;
            return *(this.address + this.pos++);
        }
    }
}
