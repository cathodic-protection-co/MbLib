using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using RJCP.IO.Ports;

namespace MbCmd
{
    class BaseOptions
    {
        [Option('v', "verbose", HelpText = "Display extra information.")]
        public bool Verbose { get; set; }

        [Option('p', "port", Required = true, HelpText = "Port name to use (e.g. 'COM5').")]
        public string PortName { get; set; }

        [Option('b', "baud", Default = 19200, HelpText = "Baud rate to use.")]
        public int BaudRate { get; set; }

        [Option('f', "format", Default = "8N1", HelpText = "Data format to use.")]
        public string DataFormat { get; set; }

        public int DataBits
        {
            get
            {
                if (DataFormat.Length != 3)
                    throw new FormatException("Invalid data format string.");
                if (!int.TryParse(DataFormat.Substring(0, 1), out int value))
                    throw new FormatException("Invalid data format string.");
                return value;
            }
        }

        public Parity Parity
        {
            get
            {
                if (DataFormat.Length != 3)
                    throw new FormatException("Invalid data format string.");
                char c = DataFormat.ToUpper()[1];
                if (c == 'N')
                    return Parity.None;
                else if (c == 'E')
                    return Parity.Even;
                else if (c == 'O')
                    return Parity.Odd;
                else if (c == 'M')
                    return Parity.Mark;
                else if (c == 'S')
                    return Parity.Space;
                else
                    throw new FormatException("Invalid data format string.");
            }
        }

        public StopBits StopBits
        {
            get
            {
                if (DataFormat.Length != 3)
                    throw new FormatException("Invalid data format string.");
                if (!int.TryParse(DataFormat.Substring(2, 1), out int value))
                    throw new FormatException("Invalid data format string.");
                if (value == 1)
                    return StopBits.One;
                else if (value == 2)
                    return StopBits.Two;
                else
                    throw new FormatException("Invalid data format string.");
            }
        }
    }
}
