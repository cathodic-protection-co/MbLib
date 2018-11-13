using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace MbCmd
{
    [Verb("read3", HelpText = "Issue 'Read Holding Registers 0x03' command.")]
    class Read3Options : CommandOptions
    {
        [Option('c', "count", Default = (ushort)1, HelpText = "Register count.")]
        public ushort RegisterCount { get; set; }
    }
}
