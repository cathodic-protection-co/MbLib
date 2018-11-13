using System;
using System.Linq;
using RJCP.IO.Ports;
using CommandLine;
using MbLib;
using System.IO;
using System.Collections.Generic;

namespace MbCmd
{
    class Program
    {
        static int Main(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<Read3Options, Read4Options, Write6Options, Write16Options, InteractiveOptions, DetectPortsOptions>(args)
                .MapResult(
                    (Read3Options opts) => RunRead3(opts),
                    (Read4Options opts) => RunRead4(opts),
                    (Write6Options opts) => RunWrite6(opts),
                    (Write16Options opts) => RunWrite16(opts),
                    (InteractiveOptions opts) => RunInteractive(opts),
                    (DetectPortsOptions opts) => RunDetectPorts(opts),
                    (ListPortsOptions opts) => RunListPorts(opts),
                    errs => 1
                );
        }

        static bool OpenPort(ModbusRtuClient client)
        {
            try
            {
                client.Open();
            }
            catch (Exception exp) when (
                exp is UnauthorizedAccessException ||
                exp is InvalidOperationException ||
                exp is IOException)
            {
                Console.Error.WriteLine("Unable to open port.");
                return false;
            }
            return true;
        }

        static int HandleException(Exception exp)
        {
            if (exp is TimeoutException)
            {
                Console.Error.WriteLine("Timeout.");
                return 2;
            }
            else if (exp is FormatException)
            {
                Console.Error.WriteLine("Frame Error.");
                return 3;
            }
            else if (exp is ModbusException)
            {
                switch ((exp as ModbusException).ExceptionCode)
                {
                    case 1:
                        Console.Error.WriteLine("Modbus exception: Illegal Function.");
                        break;
                    case 2:
                        Console.Error.WriteLine("Modbus exception: Illegal Data Address.");
                        break;
                    case 3:
                        Console.Error.WriteLine("Modbus exception: Illegal Data Value");
                        break;
                    default:
                        Console.Error.WriteLine($"Modbus exception: code {(exp as ModbusException).ExceptionCode}");
                        break;
                }
                return 4;
            }
            else
            {
                throw exp;
            }
        }

        static int RunRead3(Read3Options opts)
        {
            using (var client = new ModbusRtuClient())
            {
                client.PortName = opts.PortName;
                client.BaudRate = opts.BaudRate;
                client.DataBits = opts.DataBits;
                client.Parity = opts.Parity;

                if (!OpenPort(client))
                    return 1;

                ushort[] values;
                try
                {
                    values = client.ReadHoldingRegisters(opts.UnitAddress, opts.RegisterAddress, opts.RegisterCount);
                }
                catch (Exception exp)
                {
                    return HandleException(exp);
                }

                for (int i = 0; i < values.Length; i++)
                {
                    Console.Out.Write("0x");
                    Console.Out.Write(values[i].ToString("X4"));
                    Console.Out.Write("    ");
                    Console.Out.WriteLine(values[i].ToString("####0"));
                }

                return 0;
            }
        }

        static int RunRead4(Read4Options opts)
        {
            using (var client = new ModbusRtuClient())
            {
                client.PortName = opts.PortName;
                client.BaudRate = opts.BaudRate;
                client.DataBits = opts.DataBits;
                client.Parity = opts.Parity;

                if (!OpenPort(client))
                    return 1;

                ushort[] values;
                try
                {
                    values = client.ReadInputRegisters(opts.UnitAddress, opts.RegisterAddress, opts.RegisterCount);
                }
                catch (Exception exp)
                {
                    return HandleException(exp);
                }

                for (int i = 0; i < values.Length; i++)
                {
                    Console.Out.Write("0x");
                    Console.Out.Write(values[i].ToString("X4"));
                    Console.Out.Write("    ");
                    Console.Out.WriteLine(values[i].ToString("####0"));
                }

                return 0;
            }
        }

        static int RunWrite6(Write6Options opts)
        {
            using (var client = new ModbusRtuClient())
            {
                client.PortName = opts.PortName;
                client.BaudRate = opts.BaudRate;
                client.DataBits = opts.DataBits;
                client.Parity = opts.Parity;

                if (!OpenPort(client))
                    return 1;

                try
                {
                    client.WriteHoldingRegister(opts.UnitAddress, opts.RegisterAddress, opts.Value);
                }
                catch (Exception exp)
                {
                    return HandleException(exp);
                }

                return 0;
            }
        }

        static int RunWrite16(Write16Options opts)
        {
            using (var client = new ModbusRtuClient())
            {
                client.PortName = opts.PortName;
                client.BaudRate = opts.BaudRate;
                client.DataBits = opts.DataBits;
                client.Parity = opts.Parity;

                if (!OpenPort(client))
                    return 1;

                client.WriteMultipleHoldingRegisters(opts.UnitAddress, opts.RegisterAddress, opts.RegisterValues.ToArray());
                return 0;
            }
        }

        static int RunInteractive(InteractiveOptions opts)
        {
            using (var client = new ModbusRtuClient())
            {
                client.PortName = opts.PortName;
                client.BaudRate = opts.BaudRate;
                client.DataBits = opts.DataBits;
                client.Parity = opts.Parity;

                if (!OpenPort(client))
                    return 1;

                var parser = new InteractiveCommandParser(client);
                bool close = false;
                do
                {
                    Console.Error.Write("] ");
                    var line = Console.In.ReadLine();
                    try
                    {
                        close = parser.ProcessCommand(line);
                    }
                    catch (Exception exp)
                    {
                        HandleException(exp);
                    }
                    Console.Out.WriteLine();
                }
                while (!close);
            }
            return 0;
        }

        static int RunDetectPorts(DetectPortsOptions opts)
        {
            var validPorts = new List<PortDescription>();
            using (var client = new ModbusRtuClient())
            {
                client.BaudRate = opts.BaudRate;
                client.DataBits = opts.DataBits;
                client.Parity = opts.Parity;

                foreach (var portDesc in SerialPortStream.GetPortDescriptions())
                {
                    client.PortName = portDesc.Port;
                    if (opts.Verbose)
                    {
                        Console.Error.Write($"Trying {portDesc.Port}");
                        if (!string.IsNullOrEmpty(portDesc.Description))
                            Console.Out.Write($" ({portDesc.Description})");
                        Console.Out.Write("...");
                    }
                    try
                    {
                        client.Open();
                    }
                    catch (Exception exp ) when (exp is UnauthorizedAccessException || exp is InvalidOperationException)
                    {
                        if (opts.Verbose)
                            Console.Error.WriteLine("already in use.");
                        continue;
                    }
                    catch (IOException)
                    {
                        if (opts.Verbose)
                            Console.Error.WriteLine("cannot open.");
                        continue;
                    }

                    try
                    {
                        client.ReadHoldingRegisters(opts.UnitAddress, 0, 1);
                    }
                    catch (TimeoutException)
                    {
                        if (opts.Verbose)
                            Console.Error.WriteLine("timeout.");
                        client.Close();
                        continue;
                    }
                    catch (FormatException)
                    {
                        if (opts.Verbose)
                            Console.Error.WriteLine("frame error.");
                        client.Close();
                        continue;
                    }
                    catch (ModbusException)
                    {
                    }
                    if (opts.Verbose)
                        Console.Error.WriteLine("valid.");
                    validPorts.Add(portDesc);
                    client.Close();
                }
            }
            if (opts.Verbose)
                Console.Error.WriteLine();
            foreach (var portDesc in validPorts)
            {
                Console.Out.Write($"{portDesc.Port}");
                if (!string.IsNullOrEmpty(portDesc.Description))
                    Console.Out.Write($" ({portDesc.Description})");
                Console.Out.WriteLine();
            }

            return 0;
        }

        static int RunListPorts(ListPortsOptions opts)
        {
            foreach (var portDesc in SerialPortStream.GetPortDescriptions())
            {
                Console.Out.Write($"{portDesc.Port}");
                if (!string.IsNullOrEmpty(portDesc.Description))
                    Console.Out.Write($" ({portDesc.Description})");
                Console.Out.WriteLine();
            }
            return 0;
        }
    }
}
