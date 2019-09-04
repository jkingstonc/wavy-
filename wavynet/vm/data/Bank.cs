/*
 * James Clarke
 * 22/08/19
 */

using System.Collections.Generic;

namespace wavynet.vm.data
{
    // A Bank instance is a storage format for data in a wavy runtime instance
    public class Bank
    {
        // Bank items are referenced via an integer id
        private Dictionary<int, WItem> bank_dict;
        private Type type;

        public Bank(Type type)
        {
            this.bank_dict = new Dictionary<int, WItem>();
            this.type = type;
        }

        public void add(int id, WItem item)
        {
            this.bank_dict.Add(id, item);
        }

        public void set(int id, WItem item)
        {
            this.bank_dict[id] = item;
        }

        public Bank.Type get_type()
        {
            return this.type;
        }

        public WItem get_item(int id)
        {
            return this.bank_dict[id];
        }

        public bool contains(int id)
        {
            return this.bank_dict.ContainsKey(id);
        }


        // Indicates the type of Bank
        // MBank stores Class, Func & Var definitions
        // LBank stores literal occurances
        public enum Type
        {
            MBank,
            CBank,
        }
    }
}