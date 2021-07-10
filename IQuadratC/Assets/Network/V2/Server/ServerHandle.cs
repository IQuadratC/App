using Network.V2.Both;
using UnityEngine;

namespace Network.V2.Server
{
    public class ServerHandle
    {
        private Server server;
        public ServerHandle(Server server)
        {
            this.server = server;
        }
        
        public void ClientConnectionRecived(ServerClient fromClient, Packet packet)
        {
            server.clients[fromClient.id].state = NetworkState.connected;
            Debug.Log($"SERVER: Client {fromClient.id} connected.");
        }
        public void DebugMessage(ServerClient fromClient, Packet packet)
        {
            string message = packet.ReadString();
            Debug.Log("SERVER: Client "+ fromClient.id +" Debug: " + message);
        }
    }
}