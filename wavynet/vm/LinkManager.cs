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
    public class LinkManager : VMComponent
    {
        // Store assemblies
        public Dictionary<string, Assembly> assemblies;

        public LinkManager()
        {
            this.component_id = "LinkManager";
            this.assemblies = new Dictionary<string, Assembly>();
        }

        public override void setup()
        {
            base.setup();
            bind_all_dll();
        }

        // Bind all dlls
        public void bind_all_dll()
        {
            Wavy.logger.log("[" + this.component_id + "] " + "binding all dlls");
            foreach (KeyValuePair<string, string> dll in this.get_all_dlls())
                bind_dll(dll.Key, dll.Value);
        }

        // This gets every dll file within the "native/dll/" installation folder
        private Dictionary<string, string> get_all_dlls()
        {
            return new Dictionary<string, string>() {
                //{ "TestFile", @"F:\OneDrive - Lancaster University\programming\c#\DLLTest\bin\Debug\netstandard2.0\DLLTest.dll" },
                { "TestFile", @"C:\Users\44778\OneDrive - Lancaster University\programming\c#\DLLTest\bin\Debug\netstandard2.0\DLLTest.dll" },
            };
        }

        // Bind the DLL for the native code that we want to use
        private void bind_dll(string name, string path)
        {
            try
            {
                this.assemblies.Add(name, Assembly.LoadFile(path));
            }
            catch
            {
                ASSERT_ERR(true, VMErrorType.INVALID_CORE_COUNT, "couldn't load Assembly '"+path+"'");
            }
        }
    }
}
