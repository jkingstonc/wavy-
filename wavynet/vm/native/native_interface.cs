/*
 * James Clarke
 * 29/08/19
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace wavynet.vm.native
{
    /*
     * Class used to handle & dispatch native method calls from the vm
     */
    class NativeInterface
    {
        private VM vm;
        public NativeEnv env;

        // Store assemblies
        private Dictionary<string, Assembly> assemblies;

        public NativeInterface(VM vm)
        {
            this.vm = vm;
            this.env = new NativeEnv(this.vm);
            this.assemblies = new Dictionary<string, Assembly>();
        }

        // Call a native function, and pass in the NativeEnviroment in use
        public void call_native_func(string dll_path, string func, object[] args)
        {
            // If we dont have the dll assembly, bind it
            if(!this.assemblies.ContainsKey(dll_path))
                bind_dll(dll_path);
            // Loop over each class definition (Type) and attempt to call the func
            // Note, we should explicitly check for the correct type
            foreach (Type type in this.assemblies[dll_path].GetExportedTypes())
            {
                var c = Activator.CreateInstance(type);
                Console.WriteLine(type.InvokeMember(func, BindingFlags.InvokeMethod, null, c, args));
            }
        }

        // Bind the DLL for the native code that we want to use
        private void bind_dll(string dll_path)
        {
            this.assemblies.Add(dll_path, Assembly.LoadFile(dll_path));
        }
    }
}
