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
        public ItemType type;

        public WavyItem(object value, ItemType type)
        {
            this.value = value;
            this.type = type;
        }
    }

    public enum ItemType
    {
        BOOL,
        INT,
        DOUBLE,
        STRING,
        FUNC,
        OBJECT,
        CLASS,
    }
}
