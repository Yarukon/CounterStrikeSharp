using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Memory.Interop.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class MemHookAttribute : Attribute
    {
        public string Pattern { get;}
        public string DetourName { get; }
        public string ConfigName { get; }
        public CModule Module { get; }

        /// <summary>
        /// MemHookAttribute
        /// </summary>
        /// <param name="pattern">The Pattern you wanna search, this will be config entry name when configName is not empty</param>
        /// <param name="detourName">Target detour function name</param>
        /// <param name="configName">Will load from gamedata/plugins/(configName).json when defined</param>
        /// <param name="module">The module you wanna search in</param>
        public MemHookAttribute(string pattern, string detourName, string configName = "", CModule module = CModule.SERVER)
        {
            Pattern = pattern;
            DetourName = detourName;
            ConfigName = configName;
            Module = module;
        }
    }
}
