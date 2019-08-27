/*
 * James Clarke
 * 22/08/19
 */
using System.Collections.Generic;

namespace wavynet.vm.data
{
    // BankManager controls muli-core access to Bank data
    public class BankManager
    {
        public void setup_l_test()
        {
            this.l_bank.add_test();
            if (VM.MULTI_CORE)
                this.l_lock.Add(0, new ItemLock());
        }

        // The data banks this vm uses
        public Bank m_bank, l_bank;
        // The item locks that this vm uses
        public Dictionary<int, ItemLock> m_lock, l_lock = null;

        public BankManager()
        {
            this.m_bank = new Bank(Bank.Type.MBank);
            this.l_bank = new Bank(Bank.Type.LBank);

            if(VM.MULTI_CORE)
            {
                m_lock = new Dictionary<int, ItemLock>();
                l_lock = new Dictionary<int, ItemLock>();
            }
        }


        // Used when an instruction wants to request access to a bank item
        // The request will only be granted if the item is available
        public WavyItem request_item(Core core, Bank.Type bank_type, int id)
        {
            Bank bank = get_bank_type(bank_type);
            Dictionary<int, ItemLock> bank_lock = get_lock_type(bank_type);

            core.ASSERT_ERR(!bank.contains(id), ErrorType.INVALID_BANK_ID, "Bank item doesn't exist: "+id);

            // Dealing with multi threading
            if (VM.MULTI_CORE)
            {
                // First request use of the item
                if (bank_lock[id].request_use(core.state.id))
                {
                    return bank.get_item(id);
                }
                // If we are denied the request, we are in a BLOCKED state
                else
                {
                    core.state.multi_core_state = MultiCoreState.BLOCKED;
                    core.handle_block();
                }
            }
            // Dealing with a single thread
            else
            {
                // We don't need to check locks, as it impossible to encounter concurrent locks
                return bank.get_item(id);
            }
            return null;
        }

        // Called when the core is done with a bank item
        public void release_item(Core core, Bank.Type bank_type, int id)
        {
            if(VM.MULTI_CORE)
            {
                Bank bank = get_bank_type(bank_type);
                Dictionary<int, ItemLock> bank_lock = get_lock_type(bank_type);

                core.ASSERT_ERR(!bank.contains(id), ErrorType.INVALID_BANK_ID, "Bank item doesn't exist, so can't be released: " + id);
                bank_lock[id].release_use();
            }
        }

        // Get the correct bank given the type
        private Bank get_bank_type(Bank.Type bank_type)
        {
            if (bank_type == Bank.Type.MBank)
                return this.m_bank;
            else if (bank_type == Bank.Type.LBank)
                return this.l_bank;
            return null;
        }

        // Get the correct bank lock given the type
        private Dictionary<int, ItemLock> get_lock_type(Bank.Type bank_type)
        {
            if (bank_type == Bank.Type.MBank)
                return this.m_lock;
            else if (bank_type == Bank.Type.LBank)
                return this.l_lock;
            return null;
        }
    }


    public class ItemLock
    {
        private bool locked = false;
        // The core that is using this lock
        private int core_id = -1;

        // When a core wants access to the item
        public bool request_use(int core_id)
        {
            if (!this.locked)
            {
                this.locked = true;
                this.core_id = core_id;

                return true;
            }
            return false;
        }

        public void release_use()
        {
            this.locked = false;
            this.core_id = -1;
        }
    }
}