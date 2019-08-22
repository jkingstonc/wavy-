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
        public const bool MULTI_CORE = true;

        // FOR DEBUGGING
        public const bool INSTR_DEBUG = true;
        public const bool TRACE_DEBUG = true;
        

        public VM()
        {
            this.bank_manager = new BankManager();

            BytecodeInstance[] sequence = new BytecodeInstance[]
            {
                new BytecodeInstance(0x2),
            };
            Core test_core = new Core(this);
            test_core.register_bytecode_seq(sequence);
            test_core.evaluate_sequence();
        }
    }
}
