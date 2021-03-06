# MbLib
Modbus library and command line tools

This is a basic, portable, Modbus library with associated command line tools, written for .NET Core.

## Contents
1. [Installation](#installation)
1. [Command Line Usage](#command-line-usage)
1. [Interactive Session Usage](#interactive-session-usage)
1. [Library Usage](#library-usage)
1. [Dependencies](#dependencies)
1. [Road Map](#road-map)

## Installation
Run the appropriate installer for your platform (only Windows x86 and Window x64 installers are currently available, but Linux packages will be available in the future).

On Windows, the install directory should be automatically added to the users PATH environment variable. A restart may be required.

## Command Line Usage
The `MbCmd` executable provides a scriptable interface for the library. Usage is as follows:

```
mbcmd <verb> <options>
```

with the following supported verbs:

`read3` - Send a single 'Read Holding Registers' command to a device. ([link](#read3))

`read4` - Send a single 'Read Input Registers' command to a device. ([link](#read4))

`write6` - Send a single 'Write Holding Register' command to a device. ([link](#write6))

`write16` - Send a single 'Write Multiple Holding Registers' command to a device. ([link](#read16))

`interactive` - Start an interactive session on the specified port. ([link](#interactive))

`detect-ports` - List all ports connected to Modbus networks. ([link](#detect-ports))


### read3
*Read Holding Registers*

Usage:

```
mbcmd read3 --port <port_name> [options]
```

`--port <port_name>` - The port to connect to. On Windows this will be in the form `COMX` where `X` is the assigned port number. On Linux this will be the serial port device file, e.g. `/dev/ttySX`.

`--baud <baud_rate>` - The baud rate for the port (default: `19200`).

`--format <format>` - The data format (default: `8N1`). Only `8` data bits are currently supported. Parity modes supported are `N` (none), `E` (even), `O` (odd), `M` (mark) and `S` (space). `1` or `2` stop bits are supported.

`--unitadr <unit_adr>` - The Modbus device address (default: `1`).

`--regadr <register_address>` - The first register address to read (default: `1`).

`--count <register_count>` - The number of sequential registers to read (default: `1`).

### read4
*Read Input Registers*

Usage:

```
mbcmd read4 --port <port_name> [options]
```

`--port <port_name>` - The port to connect to. On Windows this will be in the form `COMX` where `X` is the assigned port number. On Linux this will be the serial port device file, e.g. `/dev/ttySX`.

`--baud <baud_rate>` - The baud rate for the port (default: `19200`).

`--format <format>` - The data format (default: `8N1`). Only `8` data bits are currently supported. Parity modes supported are `N` (none), `E` (even), `O` (odd), `M` (mark) and `S` (space). `1` or `2` stop bits are supported.

`--unitadr <unit_adr>` - The Modbus device address (default: `1`).

`--regadr <register_address>` - The first register address to read (default: `0`).

`--count <register_count>` - The number of sequential registers to read (default: `1`).

### write6
*Write Holding Register*

Usage:

```
mbcmd write6 --port <port_name> --value <value> [options]
```

`--port <port_name>` - The port to connect to. On Windows this will be in the form `COMX` where `X` is the assigned port number. On Linux this will be the serial port device file, e.g. `/dev/ttySX`.

`--value <value>` - The 16-bit unsigned value to write to the register.

`--baud <baud_rate>` - The baud rate for the port (default: `19200`).

`--format <format>` - The data format (default: `8N1`). Only `8` data bits are currently supported. Parity modes supported are `N` (none), `E` (even), `O` (odd), `M` (mark) and `S` (space). `1` or `2` stop bits are supported.

`--unitadr <unit_adr>` - The Modbus device address (default: `1`).

`--regadr <register_address>` - The register address to write (default: `0`).

`--count <register_count>` - The number of sequential registers to read (default: `1`).

### write16
*Write Multiple Holding Registers*

Usage:

```
mbcmd write16 --port <port_name> --value <value> [options]
```

`--port <port_name>` - The port to connect to. On Windows this will be in the form `COMX` where `X` is the assigned port number. On Linux this will be the serial port device file, e.g. `/dev/ttySX`.

`--values <value>` - The 16-bit unsigned values to write to the registers, seperated by spaces.

`--baud <baud_rate>` - The baud rate for the port (default: `19200`).

`--format <format>` - The data format (default: `8N1`). Only `8` data bits are currently supported. Parity modes supported are `N` (none), `E` (even), `O` (odd), `M` (mark) and `S` (space). `1` or `2` stop bits are supported.

`--unitadr <unit_adr>` - The Modbus device address (default: `1`).

`--regadr <register_address>` - The first register address to write (default: `0`).

### interactive
*Start an interactive session*

Usage:

```
mbcmd interactive --port <port_name> [options]
```

`--port <port_name>` - The port to connect to. On Windows this will be in the form `COMX` where `X` is the assigned port number. On Linux this will be the serial port device file, e.g. `/dev/ttySX`.

`--baud <baud_rate>` - The baud rate for the port (default: `19200`).

`--format <format>` - The data format (default: `8N1`). Only `8` data bits are currently supported. Parity modes supported are `N` (none), `E` (even), `O` (odd), `M` (mark) and `S` (space). `1` or `2` stop bits are supported.

See [Interactive Session Usage](#interactive-session-usage) for how to use this mode.


### detect-ports
*Attempts to list all serial ports which are connected to a Modbus network*

Usage:

```
mbcmd detect-ports [options]
```
`--baud <baud_rate>` - The baud rate for the port (default: `19200`).

`--format <format>` - The data format (default: `8N1`). Only `8` data bits are currently supported. Parity modes supported are `N` (none), `E` (even), `O` (odd), `M` (mark) and `S` (space). `1` or `2` stop bits are supported.

`--unitadr <unit_adr>` - The Modbus device address (default: `1`).

This command attempts to open each serial port in turn and issues a Read Holding Registers command to a device. If any valid Modbus message is received (including Modbus exception messages), then the name of the port is shown. 

## Interactive Session Usage
The interactive session supports the following commands:


`read3 <unit_adr> <reg_adr> <reg_count>` - Read holding registers.
`read4 <unit_adr> <reg_adr> <reg_count>` - Read input registers.
`write6 <unit_adr> <reg_adr> <value>` - Write holding register.
`write16 <unit_adr> <reg_adr> <value1> <value2> ...` - Write multiple holding registers.
`portinfo` - Display connection information.
`help` - Display command list.
`close` - Close the port and exit.

All parameters for each command are required (i.e. there are no assumed defaults in interactive mode).

## Library Usage
*[TODO]*

## Dependencies
* .NET Core 2.1.6 / .NET Standard 2.0
* [SerialPortStream](https://www.nuget.org/packages/SerialPortStream/) - *Thanks [jcurl](https://github.com/jcurl)*
* [CommandLineParser](https://www.nuget.org/packages/CommandLineParser/) - *Thanks [eric](https://github.com/ericnewton76)*

*Note: All installers/packages include all required dependencies.*

## Road Map
* Async support for libary.
* Add support for other Modbus variants:
  - Modbus/ASCII
  - Modbus/TCP
* Add support for running script files.
* Add `wait` command to interactive and script modes.
* Add more output formatting options.
* Add support for other standard function codes.
* Real documentation.
* Powershell cmdlets (Windows only).
* Create Linux paackages (`.deb` and `.rpm` to start)

