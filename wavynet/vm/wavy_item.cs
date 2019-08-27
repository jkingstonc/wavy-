/*
 * James Clarke
 * 22/08/19
 */

using System;

namespace wavynet.vm
{
    public class WavyItem
    {
        public object value;
        public Type type;

        public WavyItem(object value, Type type)
        {
            this.value = value;
            this.type = type;
        }
    }
}
