/*
 * James Clarke
 * 30/08/19
 */

using System;
using System.Collections.Generic;
using wavynet.file;
using wavynet.file.wcformat;
using wavynet.vm;
using wavynet.vm.core;
using wavynet.vm.data.items;

namespace wavynet
{
    class Wavy
    {
        public static Logger logger;

        static void Main(string[] args)
        {
            WCWriter.SerializeObject<WC>(
                WC.gen_WC(
                    "WATERCLOSET",
                    "test",
                    @"F:\OneDrive - Lancaster University\programming\c#\wavy-\",
                    DateTime.Now.ToString("h:mm:ss tt"),
                    true,
                    new Int32[]
                    {
                        (Int32)Opcode.LD_CONST, 1,
                        (Int32)Opcode.INVOKE_FUNC,
                        (Int32)Opcode.PRINT,
                        (Int32)Opcode.LD_CONST, 0,
                        (Int32)Opcode.PRINT,
                    },
                    new List<WItem>()
                    {
                        (Wstring)"Done!",
                        new WFunction("native_test", true, 0, 0, null),
                        (Wint)50
                    }
                ),
                @"F:\OneDrive - Lancaster University\programming\c#\wavy-\test.wc~"
            ); ; ;


            Wavy.logger = new Logger();
            WCLoader loader = new WCLoader();
            // Entry point for testing
            VM vm = new VM(WCLoader.DeSerializeObject<WC>(@"F:\OneDrive - Lancaster University\programming\c#\wavy-\test.wc~"));
            vm.setup();
            vm.start();
            Console.ReadLine();
        }
    }
}
