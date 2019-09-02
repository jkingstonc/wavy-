using System;
using System.Collections.Generic;

namespace wavynet.vm.data.items
{
    public class WavyFunction : WavyItem
    {
        public string name;
        public int args_size;
        public int locals_size;
        public Int32[] bytecode;

        // Used for native functions, if native, we need to be able to resolve the dll file
        public bool is_native;
        public string dll_name;

        public WavyFunction(string name, int args_size, int locals_size, Int32[] bytecode) : base(null, ItemType.FUNC)
        {
            this.name = name;
            this.args_size = args_size;
            this.locals_size = locals_size;
            this.bytecode = bytecode;
        }

        public WavyItem call(List<WavyItem> args)
        {
            return null;
        }
    }
}