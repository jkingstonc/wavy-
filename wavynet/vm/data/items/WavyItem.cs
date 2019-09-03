/*
 * James Clarke
 * 22/08/19
 */

using System;
using System.Xml.Serialization;
using wavynet.vm.data.items;

namespace wavynet.vm
{
    [XmlInclude(typeof(WavyFunction))]
    [Serializable]
    public class WavyItem
    {
        public object value;
        public ItemType type;

        public WavyItem(object value, ItemType type)
        {
            this.value = value;
            this.type = type;
        }

        public WavyItem()
        {

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
        NULL,
    }
}
