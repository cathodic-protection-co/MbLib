using System;
using System.Collections.Generic;
using System.Text;

namespace MbLib
{
    public enum Endianness16
    {
        AB = 0,
        BA
    }

    public enum Endianness32
    {
        ABCD = 0,
        DCBA,
        CDAB,
        BACD
    }
}
