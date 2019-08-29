/*
 * James Clarke
 * 22/08/19
 */

namespace wavynet.vm.native
{
    /*
     * This class is responsible for providing 
     * an interface to the VM from native code
     */
    class NativeEnv
    {
        private VM vm;
        private Core core;

        public NativeEnv(VM vm, Core core)
        {
            this.vm = vm;
            this.core = core;
        }

        // Close the virtual machine that the native code references
        public void close_vm()
        {
            this.vm.close();
        }
    }
}
