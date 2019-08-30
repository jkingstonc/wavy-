/*
 * James Clarke
 * 29/08/19
 */

using System;
using wavynet.vm;

namespace wavynet.profile
{
    /*
     * Represents a wc~ file as a structure
    */
    public class WCProfile
    {
        public string name;
        public BankProfile bank_profile;
        public Int32[] bytecode;

        public WCProfile(BankProfile bank_profile, Int32[] bytecode)
        {
            this.bank_profile = bank_profile;
            this.bytecode = bytecode;
        }

        public static WCProfile create_wc_profile(string path)
        {
            BankProfile b_profile = new BankProfile();
            b_profile.test_add(new WavyItem[] { new WavyItem("lol", ItemType.STRING), new WavyItem(5, ItemType.INT)});
            int count = 2;
            Int32[] sequence = new Int32[count * 6];
            for (var i = 0; i < count; i += 6)
            {
                sequence[i] = 1;
            }
            return new WCProfile(b_profile, sequence);
        }
    }
}