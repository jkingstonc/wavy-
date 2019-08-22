/*
 * James Clarke
 * 22/08/19
 */
namespace wavynet.vm.data
{
    // BankManager controls muli-core access to Bank data
    public class BankManager
    {
        // The data banks this vm uses
        public Bank m_bank, l_bank;

        public BankManager()
        {
            this.m_bank = new Bank(Bank.Type.MBank);
            this.l_bank = new Bank(Bank.Type.LBank);
        }
    }
}
