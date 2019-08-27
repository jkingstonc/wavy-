/*
 * James Clarke
 * 22/08/19
 */

using wavynet.vm.data;

namespace wavynet.vm
{
    public class VM
    {
        public BankManager bank_manager;

        // Should the vm emulate multi threading using multiple cores
        public static bool MULTI_CORE = true;

        // FOR DEBUGGING
        public static bool INSTR_DEBUG = true;
        public static bool TRACE_DEBUG = true;
        

        public VM()
        {
            this.bank_manager = new BankManager();
            this.bank_manager.setup_l_test();

            BytecodeInstance[] sequence = new BytecodeInstance[]
            {
                new BytecodeInstance(0x1),
                new BytecodeInstance(-1),
            };
            Core test_core = new Core(this);
            test_core.register_bytecode_seq(sequence);
            test_core.evaluate_sequence();
        }
    }
}
