/*
 * James Clarke
 * 29/08/19
 */

using System.Collections.Generic;
using wavynet.vm;

namespace wavynet.profile
{
    /*
     * Represents a wc~ bank profile as a structure
    */
    public struct BankProfile
    {
        public WavyItem[] l_bank_data;

        // Add a test item
        public void test_add(WavyItem[] items)
        {
            this.l_bank_data = items;
        }
    }
}
