using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MbCmd
{
    [Verb("write6", HelpText = "Issue 'Read Write Holding Register 0x06' command.")]
    class Write6Options : CommandOptions
    {
        [Option('v', "value", Required = true, HelpText = "Register value.")]
        public ushort Value { get; set; }
    }
}
