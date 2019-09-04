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
        public WavyItem call_native_func(string current_file, string func, WavyItem[] args)
        {
            // Loop over each class definition (Type) and attempt to call the func
            // Note, we should explicitly check for the correct type
            foreach (Type type in this.link_manager.assemblies[current_file].GetExportedTypes())
            {
                var c = Activator.CreateInstance(type);
                object return_value = type.InvokeMember(func, BindingFlags.InvokeMethod, null, c, args);
                // This should be optimized
                if(return_value is null)
                    return new WavyItem(ItemType.NULL);
                else if (return_value is int)
                    return new WavyItem((int)return_value, ItemType.INT);
                else if (return_value is bool)
                    return new WavyItem((bool)return_value, ItemType.BOOL);
                else if (return_value is double || return_value is float)
                    return new WavyItem((double)return_value, ItemType.DOUBLE);
                else if (return_value is string)
                    return new WavyItem((string)return_value, ItemType.STRING);
            }
            return new WavyItem(ItemType.NULL);
        }
    }
}