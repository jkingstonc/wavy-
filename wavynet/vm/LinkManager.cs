/*
 * James Clarke
 * 29/08/19
 */

using System.Collections.Generic;
using System.Reflection;
using wavynet.profile;

namespace wavynet.vm
{
    /*
     * Handles all linkage functionality for the vm
     */
    public class LinkManager
    {
        public LinkProfile link_profile;
        // Store assemblies
        public Dictionary<string, Assembly> assemblies;

        public LinkManager(LinkProfile link_profile)
        {
            this.link_profile = link_profile;
            this.assemblies = new Dictionary<string, Assembly>();
        }

        // Bind all dlls
        public void bind_all_dll()
        {
            foreach (string path in this.link_profile.dll_paths)
                bind_dll(path);
        }

        // Bind the DLL for the native code that we want to use
        private void bind_dll(string dll_path)
        {
            this.assemblies.Add(dll_path, Assembly.LoadFile(dll_path));
        }
    }
}
