using System.Net;
using UnityEngine;

namespace Network.V1.Client
{
    public class ClientHandle : MonoBehaviour
    {
        public static void ServerConnection(Packet packet)
        {
            int myId = packet.ReadInt();

            Debug.Log("CLIENT: Established TCP connection");
            V1.Client.Client.instance.clientId.value = myId;
            ClientSend.ClientConnectionReceived();

            // Now that we have the client's id, connect UDP
            V1.Client.Client.instance.udp.Connect(((IPEndPoint) V1.Client.Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        }
        
        public static void DebugMessage(Packet packet)
        {
            string message = packet.ReadString();
            Debug.Log("CLIENT: [Debug] " + message);
        }
        public static void DebugImage(Packet packet)
        {
            Texture2D texture = packet.ReadTexture2D();
        }
    }
}
