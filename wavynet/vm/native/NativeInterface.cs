/*
 * James Clarke
 * 29/08/19
 */

using System;
using System.Reflection;
using wavynet.file;
using wavynet.vm.core;

namespace wavynet.vm.native
{
    /*
     * Class used to handle & dispatch native method calls from the vm
     */
    class NativeInterface
    {
        private VM vm;
        private Core core;
        private LinkManager link_manager;
        public NativeEnv env;

        public NativeInterface(VM vm, Core core)
        {
            this.vm = vm;
            this.core = core;
            this.link_manager = vm.link_manager;
            this.env = new NativeEnv(this.vm, this.core);
        }

        // Call a native function, and pass in the NativeEnviroment in use
        public object call_native_func(string current_file, string func, WavyItem[] args)
        {
            // Loop over each class definition (Type) and attempt to call the func
            // Note, we should explicitly check for the correct type
            foreach (Type type in this.link_manager.assemblies[current_file].GetExportedTypes())
            {
                var c = Activator.CreateInstance(type);
                return type.InvokeMember(func, BindingFlags.InvokeMethod, null, c, args);
            }
            return null;
        }
    }
}