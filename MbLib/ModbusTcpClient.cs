using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace MbLib
{
    class ModbusTcpClient : ModbusClient, IDisposable
    {
        private TcpClient _client;
        private Stream _stream => _client.GetStream();

        public IPAddress IPAddress { get; set; }
        public int Port { get; set; } = 502;
        public IPEndPoint RemoteEndpoint => new IPEndPoint(IPAddress, Port);
        
        public ModbusTcpClient()
        {
            _client = new TcpClient();
        }

        public void Open()
        {
            _client.Connect(RemoteEndpoint);
        }

        public bool IsOpen => _client.Connected;

        public void Close()
        {
            _client.Close();
        }

        private void Send(byte[] buffer)
        {
            _stream.Write(buffer, 0, buffer.Length);
        }

        private void Recv(byte[] buffer, int offset, int length)
        {
            int totalRead = 0;
            int bytesRead;
            while ((bytesRead = _stream.Read(buffer, offset + totalRead, length - totalRead)) != 0)
                totalRead += bytesRead;
            if (totalRead < length)
                throw new EndOfStreamException();
        }
        
        private ushort _prevTransId = 0;
        
        private void SendHeader(int length)
        {
            var writer = new MessageBuilder();
            writer.Append(++_prevTransId);
            writer.Append((ushort)0);
            writer.Append((ushort)length);
            var header = writer.ToArray();
            Send(header);
        }

        byte[] _rxHeader = new byte[6];
        private int RecvHeader()
        {
            Recv(_rxHeader, 0, 6);
            var reader = new MessageReader(_rxHeader);
            if (reader.ReadUShort() != _prevTransId)
                throw new FormatException("Invalid Transaction ID.");
            if (reader.ReadUShort() != 0)
                throw new FormatException("Invalid Protocol ID.");
            return reader.ReadUShort();
        }

        public override int SendAndRecv(byte[] cmd, byte[] resp, out int respOffset, int maxLength = 256)
        {
            SendHeader(cmd.Length);
            Send(cmd);
            int length = RecvHeader();
            if (length > maxLength)
                throw new FormatException("Length too great.");
            Recv(resp, 0, length);
            respOffset = 0;
            return length;
        }

        public void Dispose()
        {
            if (IsOpen)
                Close();
        }
    }
}
