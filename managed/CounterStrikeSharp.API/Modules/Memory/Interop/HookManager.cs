using CounterStrikeSharp.API.Modules.Memory.Interop.Attributes;
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

#pragma warning disable 8602, 8600 // Visual Studio真是有够烦的
        public List<Guid> InitializeMemoryHooks(object self, string configName = "")
        {
            string callingAssembly = Assembly.GetCallingAssembly()?.GetName()?.Name ?? "<unknown>";
            List<Guid> guidLists = [];

            Dictionary<string, PlatformData> gameDatas = [];

            Type selfType = self.GetType();
            var fields = selfType.GetFields(Flags).Select(field => (field, field.GetCustomAttribute<MemHookAttribute>())).Where(tuple => tuple.Item2 != null);
            foreach (var (field, attribute) in fields)
            {
                string friendlyName = attribute!.DetourName.Replace("Detour", "").Replace("detour", "");

                if (!string.IsNullOrWhiteSpace(configName))
                    gameDatas = InteropGameData.ReadFrom(configName);

                MethodInfo? methodInfo = selfType.GetMethod(attribute!.DetourName, BindingFlags.Public | BindingFlags.Instance);
                if (methodInfo == null)
                {
                    Logger.LogError($"[{callingAssembly}] Failed to find detour function {attribute!.DetourName}");
                    continue;
                }

                string pattern = attribute.Pattern;
                if (!string.IsNullOrWhiteSpace(configName))
                {
                    if (gameDatas.TryGetValue(pattern, out PlatformData value))
                        pattern = IsWindows ? (string)value.Windows : (string)value.Linux;
                    else
                    {
                        Logger.LogError($"[{callingAssembly}] Hook {friendlyName} defined config name but couldn't find key {pattern}.");
                        continue;
                    }
                }

                nint address = PatternManager.Instance.FindPattern(pattern, attribute.Module);
                if (address == 0)
                {
                    Logger.LogError($"[{callingAssembly}] Failed to find pattern for function {friendlyName}");
                    continue;
                }

                Type fieldType = field.FieldType;
                if (!fieldType.IsGenericType || fieldType.GetGenericArguments().Length == 0)
                {
                    Logger.LogInformation($"[{callingAssembly}] Invalid field type for {friendlyName}");
                    continue;
                }

                Type[] genericArguments = fieldType.GetGenericArguments();

                Delegate detour = Delegate.CreateDelegate(genericArguments[0], self, methodInfo);

                Type reloadedHookType = typeof(Hook<>).MakeGenericType(genericArguments);

                object[] constructorArgs = [friendlyName, address, detour];
                object? instance = Activator.CreateInstance(reloadedHookType, constructorArgs);

                if (instance == null)
                {
                    Logger.LogError($"[{callingAssembly}] Failed to create hook instance for {friendlyName}");
                    continue;
                }

                field.SetValue(self, instance);

                instance?.GetType().GetMethod("Enable")?.Invoke(instance, null);

                Logger.LogInformation($"[{callingAssembly}] Hooked function 0x{address:X} | {friendlyName}");
                (instance as IHook)?.OverrideAssemblyName(callingAssembly);
                guidLists.Add((instance as IHook).GUID);
            }

            return guidLists;
        }
#pragma warning restore

        public Hook<T> CreateFromAddress<T>(nint address, T detour) where T : Delegate
        {
            return new Hook<T>(address, detour);
        }

        public Hook<T> CreateFromPattern<T>(string pattern, T detour, CModule module = CModule.SERVER) where T : Delegate
        {
            nint address = PatternManager.Instance.FindPattern(pattern, module);
            if (address != 0)
                return new Hook<T>(address, detour);
            else
                throw new Exception($"Failed to find pattern {pattern}");
        }

        public void DisposeTrackedHooks(IEnumerable<Guid> guids)
        {
            foreach (Guid guid in guids)
                DisposeTrackedHook(guid);
        }

        public void DisposeTrackedHook(Guid guid)
        {
            if (TrackedHooks.TryGetValue(guid, out IHook? value))
            {
                value.Dispose();
                Logger.LogInformation($"Hook [{value.GUID.ToString()}] ({value.AssemblyName}) {value!.FriendlyName} Disposed.");
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
