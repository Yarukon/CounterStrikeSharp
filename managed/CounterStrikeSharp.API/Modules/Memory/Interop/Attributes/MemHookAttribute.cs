using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Memory.Interop.Attributes
{
    /// <summary>
    /// Attribute for Hooking
    /// </summary>
    /// <param name="pattern">The Pattern you wanna search, this will be config entry name when defined configName on initializing</param>
    /// <param name="detourName">Target detour function name</param>
    /// <param name="module">The module you wanna search in</param>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class MemHookAttribute(string pattern, string detourName, CModule module = CModule.SERVER) : Attribute
    {
        public string Pattern => pattern;
        public string DetourName => detourName;
        public CModule Module => module;
    }
}
