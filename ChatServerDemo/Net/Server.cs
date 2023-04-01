using ChatClient.Net.IO;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatClient.Net
{
    class Server
    {
        TcpClient _client;
        public PacketBuilder connectPacket;
        public PacketReader packetRader;
        public event Action connectedEvent;
        public event Action messageReceivedEvent;
        public event Action disconnectEvent;

        public Server()
        {
            _client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 7865);
                packetRader = new PacketReader(_client.GetStream());
                if(!string.IsNullOrEmpty(username))
                {
                    connectPacket = new PacketBuilder();
                    connectPacket.WriteOPCode(0);
                    connectPacket.WriteMessage(username);
                    _client.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = packetRader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 5:
                            messageReceivedEvent?.Invoke();
                            break;
                        case 6:
                            messageReceivedEvent?.Invoke();
                            break;
                        case 10:
                            disconnectEvent?.Invoke();
                            break;
                        default:
                            Console.WriteLine("Reading err");
                            break;
                    }
                }
            });
        }
        public void SendMessageToServer(string message) 
        {
            var packet = new PacketBuilder();
            packet.WriteOPCode(5);
            packet.WriteMessage(message);
            _client.Client.Send(packet.GetPacketBytes());
        }
    }
}
