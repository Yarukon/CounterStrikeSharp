using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Memory.Interop.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class MemFuncAttribute : Attribute
    {
        public string Pattern { get; }
        public string ConfigName { get; }
        public CModule Module { get; }

        /// <summary>
        /// MemFunc Attribute
        /// </summary>
        /// <param name="pattern">The Pattern you wanna search, this will be config entry name when configName is not empty</param>
        /// <param name="configName">Will find from gamedata/plugins/(configName).json when defined</param>
        /// <param name="module">The module you wanna search in</param>
        public MemFuncAttribute(string pattern, string configName = "", CModule module = CModule.SERVER)
        {
            Pattern = pattern;
            ConfigName = configName;
            Module = module;
        }
    }
}
