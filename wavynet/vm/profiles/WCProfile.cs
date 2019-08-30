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
        public LinkProfile link_profile;
        public BankProfile bank_profile;
        public Int32[] bytecode;

        public WCProfile(LinkProfile link_profile, BankProfile bank_profile, Int32[] bytecode)
        {
            this.link_profile = link_profile;
            this.bank_profile = bank_profile;
            this.bytecode = bytecode;
        }

        public static WCProfile create_wc_profile(string path)
        {
            LinkProfile l_profile = new LinkProfile();
            l_profile.test_add(new string[] { @"F:\OneDrive - Lancaster University\programming\c#\DLLTest\bin\Debug\netstandard2.0\DLLTest.dll" });
            BankProfile b_profile = new BankProfile();
            b_profile.test_add(new WavyItem[] { new WavyItem("lol", typeof(string)), new WavyItem(5, typeof(int))});
            int count = 2;
            Int32[] sequence = new Int32[count * 6];
            for (var i = 0; i < count; i += 6)
            {
                sequence[i] = 1;
            }
            return new WCProfile(l_profile, b_profile, sequence);
        }
    }
}
