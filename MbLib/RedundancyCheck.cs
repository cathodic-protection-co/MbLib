using System;
using System.Collections.Generic;
using System.Text;

namespace MbLib
{
    class RedundancyCheck
    {
        public static ushort CalculateCRC(byte[] msg, int offset, int length)
        {
            ushort crc = 0xffff;
            for (int pos = offset; pos < offset + length; pos++)
            {
                crc ^= msg[pos];
                for (int i = 8; i != 0; i--)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xa001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            //Swap bytes
            return (ushort)(((crc >> 8) & 0xff) | ((crc & 0xff) << 8));
        }

        public static byte CalculateLRC(byte[] msg, int offset, int length)
        {
            int result = 0;
            for (int i = offset; i < offset + length; i++)
                result -= msg[i];
            return (byte)(result & 0xff);
        }
    }
}
