using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace MbCmd
{
    class CommandOptions : BaseOptions
    {
        [Option('u', "unitadr", Default = (byte)1, HelpText = "Modbus device address.")]
        public byte UnitAddress { get; set; }

        [Option('a', "regadr", Default = (ushort)0, HelpText = "Modbus register address.")]
        public ushort RegisterAddress { get; set; }
    }
}
