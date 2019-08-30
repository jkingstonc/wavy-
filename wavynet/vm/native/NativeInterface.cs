/*
 * James Clarke
 * 29/08/19
 */

using System;
using System.Reflection;

namespace wavynet.vm.native
{
    /*
     * Class used to handle & dispatch native method calls from the vm
     */
    class NativeInterface
    {
        private VM vm;
        private Core core;
        private LinkProfile link_profile;
        public NativeEnv env;

        public NativeInterface(VM vm, Core core)
        {
            this.vm = vm;
            this.core = core;
            this.link_profile = vm.link_profile;
            this.env = new NativeEnv(this.vm, this.core);
        }

        // Call a native function, and pass in the NativeEnviroment in use
        public object call_native_func(string dll_path, string func, object[] args)
        {
            // Loop over each class definition (Type) and attempt to call the func
            // Note, we should explicitly check for the correct type
            foreach (Type type in this.link_profile.assemblies[dll_path].GetExportedTypes())
            {
                var c = Activator.CreateInstance(type);
                return type.InvokeMember(func, BindingFlags.InvokeMethod, null, c, args);
            }
            return null;
        }
    }
}