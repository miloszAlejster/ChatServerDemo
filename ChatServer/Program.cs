using ChatServer.Net.IO;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    class Program
    {
        static List<Client> _users;
        static TcpListener _listener;
        static void Main(string[] args)
        {
            _users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7865);
            _listener.Start();

            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                
                _users.Add(client);
                BroadcastAnnoucment($"User [{client.Username}] has Connected");
                BroadcastConnection();
            }
        }
        static private void BroadcastConnection()
        {
            // user list
            foreach (var user in _users)
            {
                foreach (var userList in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOPCode(1);
                    broadcastPacket.WriteMessage(userList.Username);
                    broadcastPacket.WriteMessage(userList.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }
        static public void BroadcastMessage(string message, string uid) 
        {
            var sourceUser = _users.Where((x) => x.UID.ToString() == uid).FirstOrDefault();
            foreach(var user in _users) 
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOPCode(5);
                msgPacket.WriteMessage($"[{sourceUser.Username}]: {message}");
                user.ClientSocket.Client?.Send(msgPacket.GetPacketBytes());
            }
        }
        static public void BroadcastAnnoucment(string message)
        {
            foreach (var user in _users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOPCode(6);
                msgPacket.WriteMessage(message);
                user.ClientSocket.Client?.Send(msgPacket.GetPacketBytes());
            }
        }
        static public void BroadcastDisconnect(string UID)
        {
            var disconnectedUser = _users.Where(x => x.UID.ToString() == UID).FirstOrDefault();
            _users.Remove(disconnectedUser);
            foreach(var users in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOPCode(10);
                broadcastPacket.WriteMessage(UID.ToString());
                users.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
        }
    }
}