using System;
using System.Net;
using Xunit;
using MbLib;

namespace MbLib.Tests
{
    public class TcpClientTest
    {
        [Fact]
        public void OpenAndClose()
        {
            var client = new ModbusTcpClient();
            client.IPAddress = IPAddress.Parse("127.0.0.1");
            client.Port = 502;
            client.Open();
            Assert.True(client.IsOpen);
            client.Close();
            Assert.False(client.IsOpen);
            client.Dispose();
        }
        
        [Fact]
        public void SingleHoldingRegisterAccess()
        {
            using (var client = new ModbusTcpClient())
            {
                client.IPAddress = IPAddress.Parse("127.0.0.1");
                client.Port = 502;
                client.Open();
                client.WriteHoldingRegister(0, 2, 42);
                ushort value = client.ReadHoldingRegisters(0, 2, 1)[0];
                Assert.Equal(42, value);
                client.WriteHoldingRegister(0, 2, 0);
                ushort value2 = client.ReadHoldingRegisters(0, 2, 1)[0];
                Assert.Equal(0, value2);
            }
        }

        [Fact]
        public void MultipleHoldingRegsiterAccess()
        {
            using (var client = new ModbusTcpClient())
            {
                client.IPAddress = IPAddress.Parse("127.0.0.1");
                client.Port = 502;
                client.Open();
                ushort[] testValues = new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                client.WriteMultipleHoldingRegisters(0, 0, testValues);
                ushort[] values = client.ReadHoldingRegisters(0, 0, 10);
                for (int i = 0; i < 10; i++)
                    Assert.Equal(testValues[i], values[i]);
                client.WriteMultipleHoldingRegisters(0, 0, new ushort[10]);
                ushort[] values2 = client.ReadHoldingRegisters(0, 0, 10);
                for (int i = 0; i < 10; i++)
                    Assert.Equal(0, values2[i]);
            }
        }
    }
}
