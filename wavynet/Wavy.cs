/*
 * James Clarke
 * 30/08/19
 */

using System;
using wavynet.file;
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
            WCLoader loader = new WCLoader();
            // Entry point for testing
            VM vm = new VM(loader.load(@"F:\OneDrive - Lancaster University\programming\c#\wavy-\test.wc~"));
            vm.setup();
            vm.start();
            Console.ReadLine();
        }
    }
}
