/*
 * James Clarke
 * 29/08/19
 */

using System.Collections.Generic;
using System.Reflection;

namespace wavynet.vm.native
{
    /*
     * Handles all linkage functionality for the vm
     */
    public class LinkManager : VMComponent
    {
        // Filename & path to native dlls
        public Dictionary<string, string> dlls;
        // Store assemblies
        public Dictionary<string, Assembly> assemblies;

        public LinkManager() : base("LinkManager")
        {
            this.dlls = new Dictionary<string, string>();
            this.assemblies = new Dictionary<string, Assembly>();
        }

        public override void setup()
        {
            base.setup();
            bind_all_dll();
        }

        public void add_dll(string name, string path)
        {
            this.dlls.Add(name, path);
        }

        // Bind all dlls
        public void bind_all_dll()
        {
            LOG("binding all dlls");
            foreach (KeyValuePair<string, string> dll in this.get_all_dlls())
                bind_dll(dll.Key, dll.Value);
        }

        // This gets every dll file within the "native/dll/" installation folder
        private Dictionary<string, string> get_all_dlls()
        {
            return this.dlls;
        }

        // Bind the DLL for the native code that we want to use
        private void bind_dll(string name, string path)
        {
            this.assemblies.Add(name, Assembly.LoadFile(path+name+".dll"));
        }   
    }
}
