using System.Collections.Generic;

namespace wavynet.vm.data
{
    // A Bank instance is a storage format for data in a wavy runtime instance
    public class Bank
    {
        // Bank items are referenced via an integer id
        private Dictionary<int, WavyItem> bank_dict;
        private Bank.Type type;

        public Bank(Type type)
        {
            this.bank_dict = new Dictionary<int, WavyItem>();
            this.type = type;
        }

        public Bank.Type get_type()
        {
            return this.type;
        }

        // Indicates the type of Bank
        // MBank stores Class, Func & Var definitions
        // LBank stores literal occurances
        public enum Type
        {
            MBank,
            LBank,
        }
    }
}
