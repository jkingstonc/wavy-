/*
 * James Clarke
 * 22/08/19
 */
using System.Collections.Generic;
using wavynet.vm.core;

namespace wavynet.vm.data
{
    // BankManager controls muli-core access to Bank data
    public class BankManager : VMComponent
    {
        // The data banks this vm uses
        public Bank m_bank, c_bank;
        // The item locks that this vm uses
        public Dictionary<int, ItemLock> m_lock, c_lock = null;

        public BankManager() : base("BankManger")
        {
        }

        public void setup(WavyItem[] cbank)
        {
            base.setup();
            this.m_bank = new Bank(Bank.Type.MBank);
            this.c_bank = new Bank(Bank.Type.CBank);

            if (VM.MULTI_CORE)
            {
                m_lock = new Dictionary<int, ItemLock>();
                c_lock = new Dictionary<int, ItemLock>();
            }

            bind_lbank_data(cbank);
        }

        // Bind all the lbank data
        public void bind_lbank_data(WavyItem[] lbank)
        {
            LOG("binding all bank data from profile");
            // Loop over each value in the bank profile (M & L) & bind the data to our banks
            for (int i = 0; i < lbank.Length; i++)
            {
                this.c_bank.add(i, lbank[i]);
                if (VM.MULTI_CORE)
                {
                    c_lock.Add(i, new ItemLock());
                }
            }
        }

        // Used when an instruction wants to request access to a bank item
        // The request will only be granted if the item is available
        public WavyItem request_item(Core core, Bank.Type bank_type, int id)
        {
            Bank bank = get_bank_type(bank_type);

            core.ASSERT_ERR(!bank.contains(id), CoreErrorType.INVALID_BANK_ID, "Bank item doesn't exist: "+id);

            // Dealing with multi threading
            if (VM.MULTI_CORE)
            {
                Dictionary<int, ItemLock> bank_lock = get_lock_type(bank_type);
                // First request use of the item
                if (bank_lock[id].request_use(core.state.id))
                {
                    core.state.multi_core_state = MultiCoreState.RUNNING;
                    return bank.get_item(id);
                }
                // If we are denied the request, we are in a BLOCKED state
                else
                {
                    core.state.multi_core_state = MultiCoreState.BLOCKED;
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

        public void assign_item(Core core, int id, WavyItem item)
        {
            this.m_bank.set(id, item);
        }

        // Called when the core is done with a bank item
        public void release_item(Core core, Bank.Type bank_type, int id)
        {
            if(VM.MULTI_CORE)
            {
                Bank bank = get_bank_type(bank_type);
                Dictionary<int, ItemLock> bank_lock = get_lock_type(bank_type);

                core.ASSERT_ERR(!bank.contains(id), CoreErrorType.INVALID_BANK_ID, "Bank item doesn't exist, so can't be released: " + id);
                bank_lock[id].release_use();
            }
        }

        // Get the correct bank given the type
        private Bank get_bank_type(Bank.Type bank_type)
        {
            if (bank_type == Bank.Type.MBank)
                return this.m_bank;
            else if (bank_type == Bank.Type.CBank)
                return this.c_bank;
            return null;
        }

        // Get the correct bank lock given the type
        private Dictionary<int, ItemLock> get_lock_type(Bank.Type bank_type)
        {
            if (bank_type == Bank.Type.MBank)
                return this.m_lock;
            else if (bank_type == Bank.Type.CBank)
                return this.c_lock;
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