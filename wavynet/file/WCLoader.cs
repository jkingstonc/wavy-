/*
 * James Clarke
 * 29/08/19
 */

using System;
using System.Collections.Generic;
using wavynet.profile;
using wavynet.vm;
using wavynet.vm.data;

namespace wavynet.file
{
    /*
     * Util class for loading a wc~ file ready for the VM
     */
    class WCLoader 
    {
        private string[] lines;
        private int current = 0;
        private WCProfile profile;

        public WCProfile load(string path)
        {
            this.lines = FileIO.load_lines(path);
            validate_magic();
            this.profile.file = get_file();
            this.profile.date = get_date();
            this.profile.bytecode = get_bytecode();
            this.profile.lbank = get_lbank();
            return this.profile;
        }

        // Check we have the correct magic string
        private void validate_magic()
        {
            if(lines[current++] == "WATERCLOSET")
            {
            }
        }

        // Get the name of the source file
        private string get_file()
        {
            return lines[current++];
        }

        // Get the name of the source file
        private string get_date()
        {
            return lines[current++];
        }

        private Int32[] get_bytecode()
        {
            if(lines[current++] == "code")
            {
                List<Int32> bytecode = new List<Int32>();
                while(lines[current] != "end")
                {
                    foreach (string s in lines[current++].Split(' '))
                        bytecode.Add(Convert.ToInt32(s));
                }
                current++;
                return bytecode.ToArray();
            }
            return null;
        }

        private WavyItem[] get_lbank()
        {
            if (lines[current++] == "lbank")
            {
                List<WavyItem> items = new List<WavyItem>();
                while (lines[current] != "end")
                {
                    string type = lines[current++];
                    if(type == "wfunc")
                    {
                        do {
                            // Generate a WavyFunction here
                        } while (lines[current++] != "end");
                    }
                    else if(type == "wclass")
                    {
                        do {
                            // Generate a WavyClass here
                        } while (lines[current++] != "end");
                    }
                    else
                    {
                        var is_int = int.TryParse(type, out int i);
                        var is_double = double.TryParse(type, out double d);
                        if (is_int)
                        {
                            items.Add(new WavyItem(i, ItemType.INT));
                        }else if(is_double)
                        {
                            items.Add(new WavyItem(i, ItemType.DOUBLE));
                        }
                    }
                }
                return items.ToArray();
            }
            return null;
        }
    }
}
