using System.Threading;
using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Server
{
    public class ServerSend
    {
        private Network.Server.Server server;
        public ServerSend(Network.Server.Server server)
        {
            this.server = server;
        }
        
        public void DebugMessage(string message)
        {
            using (Packet packet = new Packet((byte) Packets.debugMessage))
            {
                packet.Write(message);
                server.SendTCPDataToAll(packet);
            }
        }
        
        public void ServerSettings(ServerClient client)
        {
            Debug.LogFormat("SERVER: [" +client.id+ "] sending settings");
            using (Packet packet = new Packet((byte) Packets.serverSettings))
            {
                packet.Write(client.id);
                packet.Write(server.serverUdpSupport);
                server.SendTCPData(client, packet);
            }
        }
        
        public void ServerStartUDP(ServerClient client)
        {
            Debug.LogFormat("SERVER: [" +client.id+ "] starting udp test");
            using (Packet packet = new Packet((byte) Packets.serverStartUDP))
            {
                server.SendTCPData(client, packet);
            }
            
            Threader.RunAsync(() =>
            {
                Thread.Sleep(2000);
                if (client.updConnected) {return;}
                ServerUDPConnection(client, false);
            });
        }
        
        public void ServerUDPConnection(ServerClient client, bool recived)
        {
            Debug.LogFormat("SERVER: [" +client.id+ "] udp test message");
            using (Packet packet = new Packet((byte) Packets.serverUDPConnection))
            {
                packet.Write(recived);

                if (recived)
                {
                    server.SendUDPData(client, packet);
                }
                else
                {
                    server.SendTCPData(client, packet);
                }
            }
        }
    }
}