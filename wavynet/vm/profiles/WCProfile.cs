/*
 * James Clarke
 * 29/08/19
 */

using System;
using wavynet.vm;
using wavynet.vm.core;
using wavynet.vm.data;

namespace wavynet.profile
{
    /*
     * Represents a wc~ file as a structure
    */
    public struct WCProfile
    {
        public string file;
        public string date;
        public WavyItem[] lbank;
        public Int32[] bytecode;
    }
}