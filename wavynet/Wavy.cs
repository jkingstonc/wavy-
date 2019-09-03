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
                    DateTime.Now.ToString("h:mm:ss tt"),
                    new Int32[]
                    {
                        (Int32)Opcode.LD_CONST, 0,
                        (Int32)Opcode.INVOKE_FUNC,
                    },
                    new List<WavyItem>()
                    {
                                new WavyFunction("testfunc", 0, 0, new Int32[] 
                                {
                                    (Int32)Opcode.PSH_ZERO,
                                    (Int32)Opcode.PRINT,
                                    (Int32)Opcode.RETURN,
                                }),
                    }
                ), 
                @"F:\OneDrive - Lancaster University\programming\c#\wavy-\testserial.wc~"
            );


            Wavy.logger = new Logger();
            WCLoader loader = new WCLoader();
            // Entry point for testing
            VM vm = new VM(WCLoader.DeSerializeObject<WC>(@"F:\OneDrive - Lancaster University\programming\c#\wavy-\testserial.wc~"));
            vm.setup();
            vm.start();
            Console.ReadLine();
        }
    }
}
