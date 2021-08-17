using Network.Both;
using Unity.Mathematics;
using UnityEngine;
using Utility;

namespace Network.Client
{
    public class ClientHandle
    {
        private Client client;
        public ClientHandle(Client client)
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
        
        public void ServerLidarStatus(Packet packet)
        {
            int status = packet.ReadInt32();
            client.lidarMode.value = status;
        }
        
        public void ServerSLAMMap(Packet packet)
        {
            int length = packet.ReadInt32();
            byte[] map = packet.ReadBytes(length);
            client.SLAMMap.value = map;
            
            Debug.Log("CLIENT: recived SLAM Map");
        }
        public void ServerPosition(Packet packet)
        {
            float3 pos = packet.ReadFloat3();
            client.position.value = pos;
            
            Debug.Log("CLIENT: recived Position: " + pos);
        }
    }
}