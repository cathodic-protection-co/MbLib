using System;

namespace MbLib
{
    public abstract class ModbusClient
    {
        public abstract int SendAndRecv(byte[] cmd, byte[] resp, out int respOffset, int maxLength = 256);
        
        public ushort[] ReadHoldingRegisters(byte unitAdr, ushort regAdr, ushort regCount, Endianness16 endianness=Endianness16.AB)
        {
            var builder = new MessageBuilder();
            builder.Append(unitAdr);
            builder.Append(0x03);
            builder.Append(regAdr);
            builder.Append(regCount);

            byte[] resp = new byte[256];
            int respLength = SendAndRecv(builder.ToArray(), resp, out int offset);
            var reader = new MessageReader(resp, offset, respLength);

            byte recvUnitAdr = reader.ReadByte();
            if (recvUnitAdr != unitAdr)
                throw new FormatException($"Unexpected unit address ({recvUnitAdr} != {unitAdr}).");

            byte recvFuncCode = reader.ReadByte();
            if ((recvFuncCode & 0x80) != 0)
                throw new ModbusException(reader.ReadByte());
                    
            if (recvFuncCode != 0x03)
                throw new FormatException($"Unexpected function code ({recvFuncCode} != 3).");

            if (reader.Count != 3 + 2 * regCount)
                throw new FormatException($"Modbus response wrong length for 0x03 ({reader.Count} != {3 + 2 * regCount}).");

            byte recvByteCount = reader.ReadByte();
            if (recvByteCount != 2 * regCount)
                throw new FormatException($"Invalid byte count ({recvByteCount} != {2 * regCount}).");

            ushort[] result = new ushort[regCount];
            for (int i = 0; i < regCount; i++)
                result[i] = reader.ReadUShort(endianness);

            return result;
        }

        public ushort[] ReadInputRegisters(byte unitAdr, ushort regAdr, ushort regCount, Endianness16 endianness = Endianness16.AB)
        {
            var builder = new MessageBuilder();
            builder.Append(unitAdr);
            builder.Append(0x04);
            builder.Append(regAdr);
            builder.Append(regCount);

            byte[] resp = new byte[256];
            int respLength = SendAndRecv(builder.ToArray(), resp, out int offset);
            var reader = new MessageReader(resp, offset, respLength);
            
            byte recvUnitAdr = reader.ReadByte();
            if (recvUnitAdr != unitAdr)
                throw new FormatException($"Unexpected unit address ({recvUnitAdr} != {unitAdr}).");

            byte recvFuncCode = reader.ReadByte();
            if ((recvFuncCode & 0x80) != 0)
                throw new ModbusException(reader.ReadByte());

            if (recvFuncCode != 0x04)
                throw new FormatException($"Unexpected function code ({recvFuncCode} != 4).");

            if (reader.Count != 3 + 2 * regCount)
                throw new FormatException($"Modbus response wrong length for 0x03 ({reader.Count} != {3 + 2 * regCount}).");

            byte recvByteCount = reader.ReadByte();
            if (recvByteCount != 2 * regCount)
                throw new FormatException($"Invalid byte count ({recvByteCount} != {2 * regCount}).");

            ushort[] result = new ushort[regCount];
            for (int i = 0; i < regCount; i++)
                result[i] = reader.ReadUShort(endianness);

            return result;
        }

        public void WriteHoldingRegister(byte unitAdr, ushort regAdr, ushort value)
        {
            var builder = new MessageBuilder();
            builder.Append(unitAdr);
            builder.Append(0x06);
            builder.Append(regAdr);
            builder.Append(value);

            byte[] resp = new byte[256];
            int respLength = SendAndRecv(builder.ToArray(), resp, out int offset);            
            var reader = new MessageReader(resp, offset, respLength);
            
            byte recvUnitAdr = reader.ReadByte();
            if (recvUnitAdr != unitAdr)
                throw new FormatException($"Unexpected unit address ({recvUnitAdr} != {unitAdr}).");

            byte recvFuncCode = reader.ReadByte();
            if ((recvFuncCode & 0x80) != 0)
                throw new ModbusException(reader.ReadByte());

            if (recvFuncCode != 0x06)
                throw new FormatException($"Unexpected function code ({recvFuncCode} != 6).");

            if (reader.Count != 6)
                throw new FormatException($"Modbus response wrong length for 0x03 ({reader.Count} != 6).");

            ushort recvRegAdr = reader.ReadUShort();
            if (recvRegAdr != regAdr)
                throw new FormatException($"Unexpected register address ({recvRegAdr} != {regAdr}).");
            
            ushort recvValue = reader.ReadUShort();
            if (recvValue != value)
                throw new FormatException($"Unexpected value ({recvValue} != {value}).");
        }

        public void WriteMultipleHoldingRegisters(byte unitAdr, ushort regAdr, ushort[] values)
        {
            WriteMultipleHoldingRegisters(unitAdr, regAdr, values, (ushort)values.Length, 0);
        }

        public void WriteMultipleHoldingRegisters(byte unitAdr, ushort regAdr, ushort[] values, ushort regCount, int offset)
        {
            var builder = new MessageBuilder();
            builder.Append(unitAdr);
            builder.Append(0x10);
            builder.Append(regAdr);
            builder.Append(regCount);
            builder.Append((byte)(2 * regCount));
            for (int i = offset; i < offset + regCount; i++)
                builder.Append(values[i]);

            byte[] resp = new byte[256];
            int respLength = SendAndRecv(builder.ToArray(), resp, out int respOffset);
            var reader = new MessageReader(resp, respOffset, respLength);
            
            byte recvUnitAdr = reader.ReadByte();
            if (recvUnitAdr != unitAdr)
                throw new FormatException($"Unexpected unit address ({recvUnitAdr} != {unitAdr}).");

            byte recvFuncCode = reader.ReadByte();
            if ((recvFuncCode & 0x80) != 0)
                throw new ModbusException(reader.ReadByte());
            
            if (recvFuncCode != 0x10)
                throw new FormatException($"Unexpected function code ({recvFuncCode} != 10).");

            if (reader.Count != 6)
                throw new FormatException($"Modbus response wrong length for 0x10 ({reader.Count} != 6).");

            ushort recvRegAdr = reader.ReadUShort();
            if (recvRegAdr != regAdr)
                throw new FormatException($"Unexpected register address ({recvRegAdr} != {regAdr}).");

            ushort recvRegCount = reader.ReadUShort();
            if (recvRegCount != regCount)
                throw new FormatException($"Unexpected value ({recvRegCount} != {regCount}).");
        }
    }
}
