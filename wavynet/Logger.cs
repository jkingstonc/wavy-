/*
 * James Clarke
 * 30/08/19
 */

using System;
using System.Collections.Generic;

namespace wavynet
{
    public class Logger
    {
        public bool LIVE_DEBUG = true;
        public List<string> logs;

        public Logger()
        {
            this.logs = new List<string>();
        }

        public void log(string msg)
        {
            string new_msg = "[" + DateTime.Now.ToString("h:mm:ss tt") + "] " + msg;
            this.logs.Add(new_msg);
            if (LIVE_DEBUG)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(new_msg);
            }
        }

        public void show()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (string log in this.logs)
                Console.WriteLine(log);
        }
    }
}