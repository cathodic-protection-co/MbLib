using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MbCmd
{
    [Verb("write16", HelpText = "Issue 'WriteMultipleHoldingRegisters 0x10' command.")]
    class Write16Options : CommandOptions
    {
        [Option('v', "values", Required = true, HelpText = "Register values (e.g. 1 2 3).")]
        public IEnumerable<ushort> RegisterValues { get; set; }
    }
}
