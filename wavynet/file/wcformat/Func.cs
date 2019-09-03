/*
 * James Clarke
 * 03/09/19
 */

using System;
using System.Collections.Generic;
using System.Text;
using wavynet.vm.data.items;

namespace wavynet.file.wcformat
{
    /*
     * Helper class that handles the formatting of WavyFunctions for disk read/write
     */
    class Func
    {
        // Take a function binary stream and return a WavyFunc definition
        public static WavyFunction parse_func()
        {
            return null;
        }

        // Take a WavyFunction and return a binary stream
        /*
                string  : name
                int     : args_size;
                int     : locals_size;
                Int32[] : bytecode;
                bool    : is_native

                // Used for native functions, if native, we need to be able to resolve the dll file
                public bool is_native;
             */
        public byte[] write_func(WavyFunction func)
        {
            byte[] name_disk = Encoding.ASCII.GetBytes(func.name);
            byte[] args_disk = BitConverter.GetBytes(func.args_size);
            byte[] locals_disk = BitConverter.GetBytes(func.locals_size);
            byte[] bytecode_disk = new byte[func.bytecode.Length * sizeof(int)];

            Buffer.BlockCopy(func.bytecode, 0, bytecode_disk, 0, bytecode_disk.Length);

            byte[] func_disk = new byte[name_disk.Length + args_disk.Length + locals_disk.Length + bytecode_disk.Length + 1];

            name_disk.CopyTo(func_disk, 0);
            args_disk.CopyTo(func_disk, name_disk.Length);
            locals_disk.CopyTo(func_disk, args_disk.Length);
            bytecode_disk.CopyTo(func_disk, locals_disk.Length);
            func_disk[func_disk.Length - 1] = (func.is_native) ? (byte)1 : (byte)0;
            return func_disk;
        }
    }
}
