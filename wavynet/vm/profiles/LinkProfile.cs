/*
 * James Clarke
 * 29/08/19
 */

namespace wavynet.profile
{
    /*
     * Represents a wc~ link profile as a structure
    */
    public struct LinkProfile
    {
        public string[] dll_paths;

        public void test_add(string[] dll_paths)
        {
            this.dll_paths = dll_paths;
        }
    }
}
