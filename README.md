# MbLib
Modbus library and command line tools

This is a basic, portable, Modbus library with associated command line tools, written for .NET Core.

## Installation
Run the appropriate installer for your platform (only Windows x86 and Window x64 installers are currently available, but Linux packages will be available in the future).

On Windows, the install directory should be automatically added to the users PATH environment variable. A restart may be required.

## Command Line Usage
The `MbCmd` executable provides a scriptable interface for the library. Usage is as follows:

```
mbcmd <verb> <options>
```

with the following supported verbs:

`read3` – Send a single ‘Read Holding Registers’ command to a device. ([link](#read3))

`read4` – Send a single ‘Read Input Registers’ command to a device. ([link](#read4))

`write6` – Send a single ‘Write Holding Register’ command to a device. ([link](#write6))

`write16` – Send a single ‘Write Multiple Holding Registers` command to a device. ([link](#read16))

`interactive` – Start an interactive session on the specified port. ([link](#interactive))

`detect-ports` – List all ports connected to Modbus networks. ([link](#detect-ports))


### read3
Usage:

```
mbcmd read3 --port <port_name> [options]
```

`--port <port_name>` – The port to connect to. On Windows this will be in the form `COMX` where `X` is the assigned port number. On Linux this will be the serial port device file, e.g. `/dev/ttySX`.

`--baud <baud_rate>` – The baud rate for the port (default: `19200`).

`--format <format>` – The data format (default: 8N1). Only `8` data bits are currently supported. Parity modes supported are `N` (none), `E` (even), `O` (odd), `M` (mark) and `S` (space). `1` or `2` stop bits are supported.

`--unitadr <unit_adr>` – The Modbus device address (default: `1`).

`--regadr <register_address> – The first register address to read (default: `1`).

`--count <register_count> – The number of sequential registers to read (default: `1`).


## Road Map
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

