using System.Net;
using SharedFiles.Utility;
using UnityEngine;

namespace Network.Client
{
    public class ClientHandle : MonoBehaviour
    {
        public static void ServerConnection(Packet packet)
        {
            int myId = packet.ReadInt();

            Debug.Log("Established TCP connection");
            Client.instance.myId = myId;
            ClientSend.ServerConnectionReceived();

            // Now that we have the client's id, connect UDP
            Client.instance.udp.Connect(((IPEndPoint) Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        }
    }
}
