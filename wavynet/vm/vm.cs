using wavynet.vm.data;

namespace wavynet.vm
{
    public class VM
    {
        // The data banks this vm uses
        public Bank m_bank, l_bank;

        // FOR DEBUGGING
        public const bool INSTR_DEBUG = true;
        public const bool TRACE_DEBUG = true;

        public VM()
        {
            this.m_bank = new Bank(Bank.Type.MBank);
            this.l_bank = new Bank(Bank.Type.LBank);

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
