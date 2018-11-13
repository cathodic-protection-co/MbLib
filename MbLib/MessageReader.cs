using System;
using System.Collections.Generic;
using System.Text;

namespace MbLib
{
    class MessageReader
    {
        private List<byte> _bytes;
        private int _rp = 0;

        public MessageReader(byte[] msg)
        {
            _bytes = new List<byte>(msg);
        }

        public MessageReader(byte[] msg, int offset=0)
        {
            _bytes = new List<byte>();
            for (int i = 0; i < msg.Length - offset; i++)
                _bytes.Add(msg[offset + i]);
        }

        public MessageReader(byte[] msg, int offset, int length)
        {
            _bytes = new List<byte>();
            for (int i = 0; i < length; i++)
                _bytes.Add(msg[offset + i]);
        }
        
        public void Reset()
        {
            _rp = 0;
        }

        public int Count => _bytes.Count;

        public byte ReadByte()
        {
            return _bytes[_rp++];
        }

        public byte ReadByte(int offset)
        {
            return _bytes[offset];
        }

        public ushort ReadUShort(Endianness16 endianness = Endianness16.AB)
        {
            ushort result = 0;
            switch (endianness)
            {
                case Endianness16.AB:
                    result |= (ushort)(_bytes[_rp++] << 8);
                    result |= _bytes[_rp++];
                    break;
                case Endianness16.BA:
                    result |= _bytes[_rp++];
                    result |= (ushort)(_bytes[_rp++] << 8);
                    break;
                default:
                    throw new ArgumentException("Invalid endianness", "endianness");
            }
            return result;
        }

        public ushort ReadUShort(int offset, Endianness16 endianness = Endianness16.AB)
        {
            ushort result = 0;
            switch (endianness)
            {
                case Endianness16.AB:
                    result |= (ushort)(_bytes[offset] << 8);
                    result |= _bytes[offset + 1];
                    break;
                case Endianness16.BA:
                    result |= _bytes[offset];
                    result |= (ushort)(_bytes[offset + 1] << 8);
                    break;
                default:
                    throw new ArgumentException("Invalid endianness", "endianness");
            }
            return result;
        }

        public short ReadShort()
        {
            return (short)ReadUShort();
        }

        public bool ValidateCRC(int payloadStart=0)
        {
            var msg = _bytes.ToArray();
            ushort calculatedCRC = RedundancyCheck.CalculateCRC(msg, payloadStart, msg.Length - payloadStart - 2);
            ushort readCRC = ReadUShort(msg.Length - payloadStart - 2);
            return calculatedCRC == readCRC;
        }

        public bool ValidateCRC(int payloadStart, int payloadLength)
        {
            var msg = _bytes.ToArray();
            ushort calculatedCRC = RedundancyCheck.CalculateCRC(msg, payloadStart, payloadLength);
            ushort readCRC = ReadUShort(payloadStart + payloadLength - 2);
            return calculatedCRC == readCRC;
        }

        public bool ValidateLRC(int payloadStart = 0)
        {
            var msg = _bytes.ToArray();
            byte calculatedLRC = RedundancyCheck.CalculateLRC(msg, payloadStart, msg.Length - payloadStart - 1);
            byte readLRC = ReadByte(msg.Length - payloadStart - 1);
            return calculatedLRC == readLRC;
        }
    }
}
