using ChatServer.Net.IO;
using System.Net.Sockets;

namespace ChatServer
{
    class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        PacketReader _packetReader;
        public Client(TcpClient client) 
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());
            
            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();

            Task.Run(() => ProcessPackets());
        }
        void ProcessPackets()
        {
            while(true) 
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch(opcode) 
                    {
                        case 5:
                            var message = _packetReader.ReadMessage();
                            Program.BroadcastMessage(message, UID.ToString());
                            break;
                        default:
                            break;
                    }
                }
                catch(Exception e) 
                {
                    Program.BroadcastDisconnect(UID.ToString());
                    Program.BroadcastAnnoucment($"User [{Username}] has Disconnected");
                    ClientSocket.Close();
                    break;
                } 
            }
        }
    }
}
