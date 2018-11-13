using System;
using System.Collections.Generic;
using System.Text;

namespace MbLib
{
    public class MessageBuilder
    {
        private List<byte> _bytes;

        public int Count => _bytes.Count;

        public MessageBuilder()
        {
            _bytes = new List<byte>();
        }

        public MessageBuilder(byte[] msg)
        {
            _bytes = new List<byte>(msg);
        }

        public MessageBuilder(byte[] msg, int offset)
        {
            _bytes = new List<byte>();
            for (int i = offset; i < msg.Length - offset; i++)
                _bytes.Add(msg[i]);
        }

        public MessageBuilder(byte[] msg, int offset, int length)
        {
            _bytes = new List<byte>();
            for (int i = offset; i < offset + length; i++)
                _bytes.Add(msg[i]);
        }

        public void Clear()
        {
            _bytes.Clear();
        }

        public byte[] ToArray()
        {
            return _bytes.ToArray();
        }

        public byte[] ToArray(int offset = 0)
        {
            return ToArray(offset, _bytes.Count - offset);
        }

        public byte[] ToArray(int offset, int length)
        {
            byte[] result = new byte[length];
            for (int i = 0; i < length; i++)
                result[i] = _bytes[offset + i];
            return result;
        }

        public void Append(byte value)
        {
            _bytes.Add(value);
        }

        public void Append(ushort value, Endianness16 endianness=Endianness16.AB)
        {
            switch (endianness)
            {
                case Endianness16.AB:
                    _bytes.Add((byte)(value >> 8));
                    _bytes.Add((byte)(value & 0xff));
                    break;
                case Endianness16.BA:
                    _bytes.Add((byte)(value & 0xff));
                    _bytes.Add((byte)(value >> 8));
                    break;
                default:
                    throw new ArgumentException("Invalid endianness", "endianness");
            }
        }

        public void Append(short value, Endianness16 endianness=Endianness16.AB)
        {
            Append((ushort)value, endianness);
        }

        public void AppendCRC(int payloadStart=0)
        {
            byte[] msg = ToArray();
            ushort crc = RedundancyCheck.CalculateCRC(msg, payloadStart, msg.Length - payloadStart);
            Append(crc);
        }

        public void AppendCRC(int payloadStart, int payloadLength)
        {
            byte[] msg = ToArray();
            ushort crc = RedundancyCheck.CalculateCRC(msg, payloadStart, payloadLength);
            Append(crc);
        }

        public void AppendLRC(int payloadStart=0)
        {
            byte[] msg = ToArray();
            byte lrc = RedundancyCheck.CalculateLRC(msg, payloadStart, msg.Length - payloadStart);
            Append(lrc);
        }

        public void AppendLRC(int payloadStart, int payloadLength)
        {
            byte[] msg = ToArray();
            byte lrc = RedundancyCheck.CalculateLRC(msg, payloadStart, payloadLength);
            Append(lrc);
        }
    }
}
