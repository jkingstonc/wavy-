using System;

namespace wavynet.vm
{
    public class WavyItem
    {
        int id;
        object value;
        Type type;

        public WavyItem(int id, object value, Type type)
        {
            this.id = id;
            this.value = value;
            this.type = type;
        }
    }
}
