using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Memory.Interop.Attributes
{
    /// <summary>
    /// Attribute for Memory patching
    /// </summary>
    /// <param name="pattern">The Pattern you wanna search, this will be config entry name when defined configName on initializing</param>
    /// <param name="bytesToPatch">The bytes you wanna patch, this will be unused when defined configName on initializing</param>
    /// <param name="module">The module you wanna search in</param>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class MemPatchAttribute(string pattern, string bytesToPatch, CModule module = CModule.SERVER) : Attribute
    {
        public string Pattern => pattern;
        public string BytesToPatch => bytesToPatch;
        public CModule Module => module;
    }
}
