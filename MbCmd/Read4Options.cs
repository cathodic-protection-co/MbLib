using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MbCmd
{
    [Verb("read4", HelpText = "Issue 'Read Input Registers 0x04' command.")]
    class Read4Options : CommandOptions
    {
        [Option('c', "count", Default = (ushort)1, HelpText = "Register count).")]
        public ushort RegisterCount { get; set; }
    }
}
