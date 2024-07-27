﻿using CounterStrikeSharp.API.Modules.Memory.Interop.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Memory.Interop
{
    public class HookManager
    {
#pragma warning disable 8618
        public static HookManager Instance { get; private set; }
#pragma warning restore

        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private ILogger Logger;

        internal Dictionary<Guid, IHook> TrackedHooks = [];

        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        internal HookManager()
        {
            if (Instance != null)
                throw new Exception("Already initialized HookManager.");

            Instance = this;
            Logger = Application.Instance.Logger;

            Logger.LogInformation("Initialized HookManager.");
        }

        /// <summary>
        /// 注册带有MemHook Attribute的钩子
        /// </summary>
        /// <param name="self">带有该Attribute的实例</param>
        /// <returns>成功注册的所有Hook GUID列表</returns>
#pragma warning disable 8602, 8600 // Visual Studio真是有够烦的
        public List<Guid> InitializeMemoryHooks(object self)
        {
            string callingAssembly = Assembly.GetCallingAssembly()?.GetName()?.Name ?? "<unknown>";
            List<Guid> guidLists = [];

            string lastRead = "";
            Dictionary<string, PlatformData> gameDatas = [];

            Type selfType = self.GetType();
            var fields = selfType.GetFields(Flags).Select(field => (field, field.GetCustomAttribute<MemHookAttribute>())).Where(tuple => tuple.Item2 != null);
            foreach (var (field, attribute) in fields)
            {
                string friendlyName = attribute!.DetourName.Replace("Detour", "").Replace("detour", "");

                if (!string.IsNullOrWhiteSpace(attribute.ConfigName))
                {
                    if (attribute.ConfigName != lastRead)
                    {
                        gameDatas = InteropGameData.ReadFrom(attribute.ConfigName);
                        lastRead = attribute.ConfigName;
                    }
                }
                else
                    lastRead = "";

                MethodInfo? methodInfo = selfType.GetMethod(attribute!.DetourName, BindingFlags.Public | BindingFlags.Instance);
                if (methodInfo == null)
                {
                    Logger.LogError($"Failed to find detour function {attribute!.DetourName}");
                    continue;
                }

                string pattern = attribute.Pattern;
                if (!string.IsNullOrWhiteSpace(lastRead))
                {
                    if (gameDatas.TryGetValue(pattern, out PlatformData value))
                        pattern = IsWindows ? (string)value.Windows : (string)value.Linux;
                    else
                    {
                        Logger.LogError($"Hook {friendlyName} defined config name but couldn't find key {pattern}.");
                        continue;
                    }
                }

                nint address = PatternManager.Instance.FindPattern(pattern, attribute.Module);
                if (address == 0)
                {
                    Logger.LogError($"Failed to find pattern for function {friendlyName}");
                    continue;
                }

                Type fieldType = field.FieldType;
                if (!fieldType.IsGenericType || fieldType.GetGenericArguments().Length == 0)
                {
                    Logger.LogInformation($"Invalid field type for {friendlyName}");
                    continue;
                }

                Type[] genericArguments = fieldType.GetGenericArguments();

                Delegate detour = Delegate.CreateDelegate(genericArguments[0], self, methodInfo);

                Type reloadedHookType = typeof(Hook<>).MakeGenericType(genericArguments);

                object[] constructorArgs = [friendlyName, address, detour];
                object? instance = Activator.CreateInstance(reloadedHookType, constructorArgs);

                if (instance == null)
                {
                    Logger.LogError($"Failed to create hook instance for {friendlyName}");
                    continue;
                }

                field.SetValue(self, instance);

                instance?.GetType().GetMethod("Enable")?.Invoke(instance, null);

                Logger.LogInformation($"Hooked function {friendlyName} -> 0x{address:X}");
                (instance as IHook)?.OverrideAssemblyName(callingAssembly);
                guidLists.Add((instance as IHook).GUID);
            }

            return guidLists;
        }
#pragma warning restore

        public Hook<T> CreateFromAddress<T>(string friendlyName, nint address, T detour) where T : Delegate
        {
            return new Hook<T>(address, detour);
        }

        public Hook<T> CreateFromPattern<T>(string friendlyName, string pattern, T detour, CModule module = CModule.SERVER) where T : Delegate
        {
            nint address = PatternManager.Instance.FindPattern(pattern, module);
            if (address != 0)
                return new Hook<T>(address, detour);
            else
                throw new Exception($"Failed to find pattern {pattern}");
        }

        public void DisposeTrackedHooks(Guid guid)
        {
            if (TrackedHooks.TryGetValue(guid, out IHook? value))
            {
                value?.Dispose();
                Logger.LogInformation($"Hook [{value!.GUID.ToString()}] ({value.AssemblyName}) {value!.FriendlyName} Disposed.");
            }
            else
                Logger.LogError($"Hook GUID {guid.ToString()} not found.");
        }

        public void ListHooks()
        {
            Logger.LogInformation("-------- H O O K S --------");
            Logger.LogInformation("AssemblyName | GUID | Address | [Enabled] Name");

            foreach (KeyValuePair<Guid, IHook> hook in TrackedHooks)
                Logger.LogInformation($"{hook.Value.AssemblyName} | {hook.Value.GUID.ToString()} | 0x{hook.Value.Address:X} | [{(hook.Value.IsEnabled ? "E" : " ")}] {hook.Value.FriendlyName}");
        }
    }
}
