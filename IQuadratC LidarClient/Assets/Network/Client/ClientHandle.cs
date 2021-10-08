using System.Linq;
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
            string[] versions = version.Split(',');
            
            if (versions.Contains("1.3"))
            {
                client.serverUdpSupport.value = packet.ReadBool();
                client.serverCamSupport.value = packet.ReadBool();
                client.serverJoystickSupport.value = packet.ReadBool();
                client.serverChatSupport.value = packet.ReadBool();
                client.serverLidarSupport.value = packet.ReadBool();
                client.serverLidarSimSupport.value = packet.ReadBool();
                client.serverSLAMMapSupport.value = packet.ReadBool();
                client.SLAMMapSize.value = packet.ReadInt32();
                client.SLAMMapIntervall.value = packet.ReadInt32();
            }
            else if (versions.Contains("1.2"))
            {
                client.serverUdpSupport.value = packet.ReadBool();
                client.serverCamSupport.value = packet.ReadBool();
                client.serverJoystickSupport.value = packet.ReadBool();
                client.serverChatSupport.value = packet.ReadBool();
                client.serverLidarSupport.value = packet.ReadBool();
                client.serverLidarSimSupport.value = packet.ReadBool();
            }
            else if (versions.Contains("1.1"))
            {
                client.serverUdpSupport.value = packet.ReadBool();
                client.serverCamSupport.value = packet.ReadBool();
                client.serverJoystickSupport.value = packet.ReadBool();
                client.serverChatSupport.value = packet.ReadBool();
                client.serverLidarSupport.value = packet.ReadBool();
            }
            
            Debug.Log("CLIENT: recived server settings:" +
                      "\nVersion " + version +
                      "\nUDP " + client.serverUdpSupport.value +
                      "\nCam " + client.serverCamSupport.value +
                      "\nJoystick " + client.serverJoystickSupport.value +
                      "\nChat " + client.serverChatSupport.value +
                      "\nLidar " + client.serverLidarSupport.value + 
                      "\nLidarSim " + client.serverLidarSimSupport.value +
                      "\nSALMMap " + client.serverSLAMMapSupport.value +
                      "\nSALMMapSize " + client.SLAMMapSize.value +
                      "\nSALMMapIntervall " + client.SLAMMapIntervall.value
                      
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
            if (length <= 0)
            {
                return;
            }
            
            byte[] map = packet.ReadBytes(length);
            client.SLAMMap.value = map;
            
            Debug.Log("CLIENT: recived SLAM Map");
        }
        public void ServerSLAMMapPart(Packet packet)
        {
            int size = (client.SLAMMapSize.value / client.SLAMMapIntervall.value) *
                       (client.SLAMMapSize.value / client.SLAMMapIntervall.value);
            if (client.SLAMMap.value == null || client.SLAMMap.value.Length != size)
            {
                client.SLAMMap.value = new byte[size];
            }
            
            int start = packet.ReadInt32();
            int end = packet.ReadInt32();

            byte[] map = packet.ReadBytes(end-start);

            for (int i = start; i < end; i++)
            {
                client.SLAMMap.value[i] = map[i-start];
            }

            Debug.LogFormat("CLIENT: recived SLAM Map Part {0}, {1}",start,end);
        }
        
        public void ServerPosition(Packet packet)
        {
            float3 pos = packet.ReadFloat3();
            client.position.value = pos;
            
            Debug.Log("CLIENT: recived Position: " + pos);
        }
        
        public void ServerGetSimulatedLidarData(Packet packet)
        {
            Debug.Log("CLIENT: Simulated Lidar Data request");
            client.clientSend.ClientSimulatedLidarData();
        }
    }
}