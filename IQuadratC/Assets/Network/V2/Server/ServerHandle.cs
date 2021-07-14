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
        
        public void DebugMessage(ServerClient fromClient, Packet packet)
        {
            string message = packet.ReadString();
            Debug.Log("SERVER: [" +fromClient.id+ "] Debug: {message}");
        }
        
        public void ClientSettings(ServerClient fromClient, Packet packet)
        {
            Debug.Log("SERVER: [" +fromClient.id+ "] recived client settings");
            fromClient.clientUdpSupport = packet.ReadBool();

            if (server.serverUdpSupport && fromClient.clientUdpSupport)
            {
                server.serverSend.ServerStartUDP(fromClient);
            }
            else
            {
                fromClient.state = NetworkState.connected;
            }
        }
        
        public void ClientUDPConnection(ServerClient fromClient, Packet packet)
        {
            fromClient.updConnected = true;
            server.serverSend.ServerUDPConnection(fromClient, true);
        }
        
        public void ClientUDPConnectionStatus(ServerClient fromClient, Packet packet)
        {
            fromClient.updConnected = packet.ReadBool() && server.serverUdpSupport;
            Debug.Log("SERVER: [" +fromClient.id+ "] UDP connection status: "+ fromClient.updConnected);
        }
    }
}