using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MbLib
{
    public class ModbusException : IOException
    {
        public byte ExceptionCode { get; set; }
        
        public ModbusException(byte exceptionCode)
        {
            ExceptionCode = exceptionCode;
        }
    }
}
