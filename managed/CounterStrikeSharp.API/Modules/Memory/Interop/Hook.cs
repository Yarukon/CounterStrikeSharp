using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CounterStrikeSharp.API.Modules.Memory.Interop
{
    public class Hook<T> : IHook where T : Delegate
    {
        private nint _funchook_t;

        private T OriginalFn;
        private nint OriginalFnPtr;

        // Keep ref
        private T Detour;
        private nint DetourFnPtr;

        public nint Address { get; private set; }
        public bool IsDisposed { get; private set; }
        public Guid GUID { get; private set; }
        public string FriendlyName { get; private set; }
        public string AssemblyName { get; private set; }

        public Hook(string detourName, nint address, T detour)
        {
            this.Address = address;
            this.Detour = detour;
            this.GUID = Guid.NewGuid();

            this.FriendlyName = detourName.Replace("Detour", "").Replace("detour", "");
            this.AssemblyName = Assembly.GetCallingAssembly()?.GetName().Name ?? "<unknown>";

            this.DetourFnPtr = Marshal.GetFunctionPointerForDelegate(Detour);
            unsafe
            {
                OriginalFnPtr = Address;
                fixed (nint* originalFnPtr = &OriginalFnPtr)
                {
                    this._funchook_t = NativeAPI.HookCreate((nint)originalFnPtr, this.DetourFnPtr);
                    if (_funchook_t != 0)
                    {
                        OriginalFn = Marshal.GetDelegateForFunctionPointer<T>(OriginalFnPtr);
                        HookManager.Instance.TrackedHooks.Add(this.GUID, this);
                    }
                    else
                        throw new Exception($"Failed to create hook for address 0x{Address:X8}");
                }
            }
        }

        // 给正常创建的途径使用的构建方法, 因为反射创建实例无法使用CallerArgumentExpression (当然正常途径也可以用上面的方法)
        public Hook(nint address, T detour, [CallerArgumentExpression(nameof(detour))] string detourName = "<unknown>") : this(detourName, address, detour)
        {
        }

        public virtual T Original
        {
            get
            {
                CheckDisposed();
                return OriginalFn;
            }
        }

        public T OriginalDisposeSafe => IsDisposed ? Marshal.GetDelegateForFunctionPointer<T>(Address) : Original;

        public virtual unsafe bool IsEnabled
        {
            get
            {
                CheckDisposed();
                unsafe
                {
                    return *(int*) _funchook_t == 1;
                }
            }
        }

        public virtual void Enable()
        {
            CheckDisposed();
            NativeAPI.HookEnable(_funchook_t);
        }

        public virtual void Disable()
        {
            CheckDisposed();
            NativeAPI.HookDisable(_funchook_t);
        }

        public virtual void Dispose()
        {
            if (this.IsDisposed)
                return;

            if (IsEnabled)
                this.Disable();

            NativeAPI.HookDestroy(_funchook_t);

            this.IsDisposed = true;

            HookManager.Instance.TrackedHooks.Remove(this.GUID);
        }

        void IHook.OverrideAssemblyName(string assemblyName) => AssemblyName = assemblyName;

        protected void CheckDisposed()
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException(message: "Hook is already disposed.", null);
        }
    }
}
