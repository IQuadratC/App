using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Client
{
    public class ClientHandle
    {
        private Network.Client.Client client;
        public ClientHandle(Network.Client.Client client)
        {
            this.client = client;
        }
        
        public void DebugMessage(Packet packet)
        {
            string message = packet.ReadString();
            Threader.RunOnMainThread(() =>
            {
                Debug.Log("CLIENT: Debug: "+ message);
            });
        }
        
        public void ServerSettings(Packet packet)
        {
            client.clientId.value = packet.ReadByte();
            
            string version = packet.ReadString();

            if (version == "1.1")
            {
                client.serverUdpSupport = packet.ReadBool();
                client.serverCamSupport.value = packet.ReadBool();
                client.serverJoystickSupport.value = packet.ReadBool();
                client.serverChatSupport.value = packet.ReadBool();
                client.serverLidarSupport.value = packet.ReadBool();
            }
            
            Debug.Log("CLIENT: recived server settings:" +
                      "\nVersion " + version +
                      "\nUDP " + client.serverUdpSupport +
                      "\nCam " + client.serverCamSupport.value +
                      "\nJoystick " + client.serverJoystickSupport.value +
                      "\nChat " + client.serverChatSupport.value +
                      "\nLidar " + client.serverLidarSupport.value
            );
            
            client.clientSend.ClientSettings();
        }
        
        public void ServerStartUDP(Packet packet)
        {
            client.udpClient.Connect();
            client.clientSend.ClientUDPConnection();
        }
        
        public void ServerUDPConnection(Packet packet)
        {
            client.clientSend.ClientUDPConnectionStatus();
        }
    }
}