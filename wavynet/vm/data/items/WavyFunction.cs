using System.Collections.Generic;

namespace wavynet.vm.data.items
{
    public class WavyFunction : WavyItem
    {
        public string name;
        private int arg_count;

        // Used for native functions, if native, we need to be able to resolve the dll file
        public bool is_native;
        public string dll_name;

        public WavyFunction(string name) : base(null, ItemType.FUNC)
        {
            this.name = name;
            this.arg_count = 0;
        }

        public int args()
        {
            return this.arg_count;
        }

        public static WavyItem make_func()
        {
            return new WavyItem(new WavyFunction("test"), ItemType.FUNC);
        }

        public WavyItem call(List<WavyItem> args)
        {
            return null;
        }
    }
}