/*
 * James Clarke
 * 03/09/19
 */

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using wavynet.vm;
using wavynet.vm.core;
using wavynet.vm.data.items;

namespace wavynet.file.wcformat
{
    /*
     *This class represents a serializable WC file
     */
    [XmlInclude(typeof(WFunction))]
    [XmlInclude(typeof(WavyClass))]
    [Serializable]
    public struct WC
    {
        public static string MAGIC_SEQUENCE = "WATERCLOSET";

        public string magic;
        public string filename;
        public string path;
        public string datetime;
        public bool require_dll;
        public Int32[] bytecode;
        public List<WavyItem> cbank;

        public static WC gen_WC(string magic, string filename, string path, string datetime, bool require_dll, Int32[] bytecode, List<WavyItem> cbank)
        {
            WC wc = new WC();
            wc.magic = magic;
            wc.filename = filename;
            wc.path = path;
            wc.datetime = datetime;
            wc.require_dll = require_dll;
            wc.bytecode = bytecode;
            wc.cbank = cbank;
            return wc;
        }
    }
}
