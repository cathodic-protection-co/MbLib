using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RJCP.IO.Ports;

namespace MbLib
{
    public class ModbusRtuClient : ModbusClient, IDisposable
    {
        private SerialPortStream _stream;

        public string PortName
        {
            get
            {
                return _stream.PortName;
            }
            set
            {
                _stream.PortName = value;
            }
        }

        public int BaudRate
        {
            get
            {
                return _stream.BaudRate;
            }
            set
            {
                _stream.BaudRate = value;
            }
        }

        public int DataBits
        {
            get
            {
                return _stream.DataBits;
            }
            set
            {
                _stream.DataBits = value;
            }
        }

        public StopBits StopBits
        {
            get
            {
                return _stream.StopBits;
            }
            set
            {
                _stream.StopBits = value;
            }
        }

        public Parity Parity
        {
            get
            {
                return _stream.Parity;
            }
            set
            {
                _stream.Parity = value;
            }
        }

        public Handshake Handshake
        {
            get
            {
                return _stream.Handshake;
            }
            set
            {
                _stream.Handshake = value;
            }
        }

        public ModbusRtuClient()
        {
            _stream = new SerialPortStream();
            var defaultPortName = SerialPortStream.GetPortNames().FirstOrDefault();
            if (defaultPortName != null)
                _stream.PortName = defaultPortName;
            BaudRate = 19200;
            Parity = Parity.None;
            StopBits = StopBits.One;
            Handshake = Handshake.None;
        }

        public void Open()
        {
            _stream.Open();
        }

        public bool IsOpen => _stream.IsOpen;

        public void Close()
        {
            _stream.Close();
        }

        private void Send(byte[] cmd)
        {
            _stream.Write(cmd, 0, cmd.Length);
            _stream.Flush();
        }

        private int Recv(byte[] resp, int maxLength)
        {
            //Read first bytes(s)
            _stream.ReadTimeout = 1000;
            int totalRead = _stream.Read(resp, 0, maxLength);
            if (totalRead == 0)
                throw new TimeoutException();
            _stream.ReadTimeout = 5;
            while (totalRead < maxLength)
            {
                //Read remaining bytes
                int bytesRead = _stream.Read(resp, totalRead, maxLength - totalRead);
                if (bytesRead == 0)
                    break;
                totalRead += bytesRead;
            }
            return totalRead;
        }

        public override int SendAndRecv(byte[] cmd, byte[] resp, out int respOffset, int maxLength = 256)
        {
            var builder = new MessageBuilder(cmd);
            builder.AppendCRC();
            var sendBytes = builder.ToArray();
            Send(sendBytes);
            int length = Recv(resp, maxLength);
            respOffset = 0;
            var reader = new MessageReader(resp);
            if (reader.Count < 4)
                throw new FormatException($"Modbus frame too short ({reader.Count} < 4).");
            if (!reader.ValidateCRC())
                throw new FormatException($"Invalid CRC.");
            return length - 2;
        }

        public void Dispose()
        {
            if (!_stream.IsDisposed)
                _stream.Dispose();
        }
    }
}
