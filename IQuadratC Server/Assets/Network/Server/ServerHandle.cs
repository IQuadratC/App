using Network.Both;
using Unity.Mathematics;
using UnityEngine;

namespace Network.Server
{
    public class ServerHandle
    {
        private Network.Server.Server server;
        public ServerHandle(Network.Server.Server server)
        {
            this.server = server;
        }
        
        public void DebugMessage(ServerClient fromClient, Packet packet)
        {
            string message = packet.ReadString();
            Debug.Log("SERVER: [" +fromClient.id+ "] Debug: " + message);
        }
        
        public void ClientSettings(ServerClient fromClient, Packet packet)
        {
            Debug.Log("SERVER: [" +fromClient.id+ "] recived client settings");
            fromClient.clientUdpSupport = packet.ReadBool();
            fromClient.clientCamSupport = packet.ReadBool();
            fromClient.clientJoystickSupport = packet.ReadBool();

            if (server.udpSupport.value && fromClient.clientUdpSupport)
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
            fromClient.updConnected = packet.ReadBool() && server.udpSupport;
            Debug.Log("SERVER: [" +fromClient.id+ "] UDP connection status: "+ fromClient.updConnected);
        }
        
        public void ClientControllMode(ServerClient fromClient, Packet packet)
        {
            int value = packet.ReadInt32();
            server.controllMode.value = value;
        }
        
        public void ClientJoystickMove(ServerClient fromClient, Packet packet)
        {
            float3 value = packet.ReadFloat3();
            server.joystickMoveEvent.Raise(value);
        }
        
        public void ClientJoystickRotate(ServerClient fromClient, Packet packet)
        {
            float value = packet.ReadFloat();
            server.joystickRotateEvent.Raise(value);
        }
        
        public void ClientJoystickStop(ServerClient fromClient, Packet packet)
        {
            server.joystickStopEvent.Raise();
        }
    }
}