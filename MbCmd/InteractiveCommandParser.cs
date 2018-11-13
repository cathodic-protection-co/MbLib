using System;
using System.Collections.Generic;
using System.Text;
using MbLib;

namespace MbCmd
{
    class InteractiveCommandParser
    {
        private ModbusRtuClient _client { get; set; }

        public InteractiveCommandParser(ModbusRtuClient client)
        {
            _client = client;
        }

        private void ProcessRead3(string[] parts)
        {
            if (parts.Length != 4)
            {
                ShowHelp("Invalid number of parameters for 'read3' command.");
                return;
            }
            if (!byte.TryParse(parts[1], out byte unitAdr))
            {
                ShowHelp("Invalid parameter [unit_address]. Must be a byte.");
                return;
            }
            if (!ushort.TryParse(parts[2], out ushort regAdr))
            {
                ShowHelp("Invalid parameter [register_address]. Must be a ushort.");
                return;
            }
            if (!ushort.TryParse(parts[3], out ushort regCount) || regCount < 1)
            {
                ShowHelp("Invalid parameter [register_count]. Must be a ushort and >= 1");
                return;
            }
            ushort[] values = _client.ReadHoldingRegisters(unitAdr, regAdr, regCount);
            for (int i = 0; i < values.Length; i++)
            {
                Console.Out.Write("0x");
                Console.Out.Write(values[i].ToString("X4"));
                Console.Out.Write("    ");
                Console.Out.WriteLine(values[i].ToString("#####"));
            }
        }

        private void ProcessRead4(string[] parts)
        {
            if (parts.Length != 4)
            {
                ShowHelp("Invalid number of parameters for 'read4' command.");
                return;
            }
            if (!byte.TryParse(parts[1], out byte unitAdr))
            {
                ShowHelp("Invalid parameter [unit_address]. Must be a byte.");
                return;
            }
            if (!ushort.TryParse(parts[2], out ushort regAdr))
            {
                ShowHelp("Invalid parameter [register_address]. Must be a ushort.");
                return;
            }
            if (!ushort.TryParse(parts[3], out ushort regCount) || regCount < 1)
            {
                ShowHelp("Invalid parameter [register_count]. Must be a ushort and >= 1");
                return;
            }
            ushort[] values = _client.ReadInputRegisters(unitAdr, regAdr, regCount);
            for (int i = 0; i < values.Length; i++)
            {
                Console.Out.Write("0x");
                Console.Out.Write(values[i].ToString("X4"));
                Console.Out.Write("    ");
                Console.Out.WriteLine(values[i].ToString("#####"));
            }
        }

        private void ProcessWrite6(string[] parts)
        {
            if (parts.Length != 4)
            {
                ShowHelp("Invalid number of parameters for 'write6' command.");
                return;
            }
            if (!byte.TryParse(parts[1], out byte unitAdr))
            {
                ShowHelp("Invalid parameter [unit_address]. Must be a byte.");
                return;
            }
            if (!ushort.TryParse(parts[2], out ushort regAdr))
            {
                ShowHelp("Invalid parameter [register_address]. Must be a ushort.");
                return;
            }
            if (!ushort.TryParse(parts[3], out ushort value))
            {
                ShowHelp("Invalid parameter [register_value]. Must be a ushort.");
                return;
            }
            _client.WriteHoldingRegister(unitAdr, regAdr, value);
        }

        private void ProcessWrite16(string[] parts)
        {
            if (parts.Length < 4)
            {
                ShowHelp("Invalid number of parameters for 'write16' command.");
                return;
            }
            if (!byte.TryParse(parts[1], out byte unitAdr))
            {
                ShowHelp("Invalid parameter [unit_address]. Must be a byte.");
                return;
            }
            if (!ushort.TryParse(parts[2], out ushort regAdr))
            {
                ShowHelp("Invalid parameter [register_address]. Must be a ushort.");
                return;
            }
            var values = new List<ushort>();
            for (int i = 3; i < parts.Length; i++)
            {
                if (!ushort.TryParse(parts[i], out ushort value))
                {
                    ShowHelp($"Invalid parameter [register_value{i - 2}]. Must be a ushort.");
                    return;
                }
                values.Add(value);
            }
            _client.WriteMultipleHoldingRegisters(unitAdr, regAdr, values.ToArray());
        }

        private void ProcessPortInfo()
        {
            Console.Out.WriteLine($"PortName:   {_client.PortName}");
            Console.Out.WriteLine($"BaudRate:   {_client.BaudRate}");
            Console.Out.WriteLine($"DataBits:   {_client.DataBits}");
            Console.Out.WriteLine($"Parity:     {_client.Parity}");
            Console.Out.WriteLine($"StopBits:   {_client.StopBits}");
        }

        private void ShowHelp(string error = null)
        {
            if (error != null)
            {
                Console.Error.WriteLine(error);
                Console.Error.WriteLine();
            }
            Console.Error.WriteLine("Valid commands are:");
            Console.Error.WriteLine("  read3   [unit_address] [register_address] [register_count]");
            Console.Error.WriteLine("  read4   [unit_address] [register_address] [register_count]");
            Console.Error.WriteLine("  write6  [unit_address] [register_address] [register_value]");
            Console.Error.WriteLine("  write16 [unit_address] [register_address] [register_value1] [register_value2] [etc]");
            Console.Error.WriteLine("  portinfo");
            Console.Error.WriteLine("  help");
            Console.Error.WriteLine("  close");
        }

        public bool ProcessCommand(string cmd)
        {
            string[] parts = cmd.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                switch (parts[0])
                {
                    case "read3":
                        ProcessRead3(parts);
                        return false;
                    case "read4":
                        ProcessRead4(parts);
                        return false;
                    case "write6":
                        ProcessWrite6(parts);
                        return false;
                    case "write16":
                        ProcessWrite16(parts);
                        return false;
                    case "portinfo":
                        ProcessPortInfo();
                        return false;
                    case "quit":
                    case "close":
                        return true;
                    case "help":
                    default:
                        ShowHelp();
                        return false;
                }
            }
            return false;
        }
    }
}