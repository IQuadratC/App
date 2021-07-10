using Network.V2.Both;
using UnityEngine;

namespace Network.V2.Server
{
    public class ServerSend
    {
        private Server server;
        public ServerSend(Server server)
        {
            this.server = server;
        }
        
        public void ServerConnection(ServerClient client)
        {
            Debug.Log($"SERVER: Sending ID to Client " + client.id + "...");
            using (Packet packet = new Packet((byte) Packets.serverConnection))
            {
                packet.Write(client.id);
                server.SendTCPData(client, packet);
            }
        }
        
        public void DebugMessage(string message)
        {
            using (Packet packet = new Packet((byte) Packets.debugMessage))
            {
                packet.Write(message);
                server.SendTCPDataToAll(packet);
            }
        }
    }
}