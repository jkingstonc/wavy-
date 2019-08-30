/*
 * James Clarke
 * 29/08/19
 */

using System.Collections.Generic;
using System.Reflection;

namespace wavynet.vm
{
    /*
     * Handles all linkage functionality for the vm
     */
    public class LinkManager
    {
        // Store assemblies
        public Dictionary<string, Assembly> assemblies;

        public LinkManager()
        {
            this.assemblies = new Dictionary<string, Assembly>();
        }

        // Bind all dlls
        public void bind_all_dll(string[] dll_paths)
        {
            foreach (string path in dll_paths)
                bind_dll(path);
        }

        // Bind the DLL for the native code that we want to use
        private void bind_dll(string dll_path)
        {
            this.assemblies.Add(dll_path, Assembly.LoadFile(dll_path));
        }
    }
}
