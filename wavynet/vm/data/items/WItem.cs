/*
 * James Clarke
 * 22/08/19
 */

using System;
using System.Xml.Serialization;
using wavynet.vm.data.items;

namespace wavynet.vm
{
    [XmlInclude(typeof(Wnull))]
    [XmlInclude(typeof(Wbool))]
    [XmlInclude(typeof(Wint))]
    [XmlInclude(typeof(Wdouble))]
    [XmlInclude(typeof(Wstring))]
    [XmlInclude(typeof(WFunction))]
    [Serializable]
    public class WItem
    {
        public dynamic value;
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
