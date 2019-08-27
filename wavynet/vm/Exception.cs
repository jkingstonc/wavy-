/*
 * James Clarke
 * 22/08/19
 */

using System;

namespace wavynet.vm
{
    // This is thrown when the core encounters a standard core error
    class CoreErrException : Exception
    {
    }

    // This is thrown when the vm encounters a standard core error
    class VMErrException : Exception
    {

    }
}