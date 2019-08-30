using System.Collections.Generic;

namespace wavynet.vm.data.items
{
    public class WavyFunction
    {
        public static WavyItem make_func()
        {
            return new WavyItem(new WavyFunction(), typeof(WavyFunction));
        }

        public WavyItem call(List<WavyItem> args)
        {
            return null;
        }
    }
}
