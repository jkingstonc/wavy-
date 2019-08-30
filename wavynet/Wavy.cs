/*
 * James Clarke
 * 30/08/19
 */

using System;
using wavynet.profile;
using wavynet.vm;

namespace wavynet
{
    class Wavy
    {
        public static Logger logger;

        static void Main(string[] args)
        {
            Wavy.logger = new Logger();
            // Entry point for testing
            VM vm = new VM();
            vm.setup("TestFile", WCProfile.create_wc_profile("TestFile"));
            vm.start();
            Console.ReadLine();
        }
    }
}
