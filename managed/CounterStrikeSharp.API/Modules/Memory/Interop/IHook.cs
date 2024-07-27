using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Memory.Interop
{
    public interface IHook : IDisposable
    {
        nint Address { get; }
        bool IsEnabled { get; }
        bool IsDisposed { get; }
        string FriendlyName { get; }
        string AssemblyName { get; }
        Guid GUID { get; }
        void OverrideAssemblyName(string assemblyName);
    }
}
