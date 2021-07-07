using UnityEngine;

namespace Network.Client
{
    public class ClientSend : MonoBehaviour
    {
        /// <summary>Sends a packet to the server via TCP.</summary>
        /// <param name="packet">The packet to send to the sever.</param>
        private static void SendTcpData(Packet packet)
        {
            packet.WriteLength();
            Client.instance.tcp.SendData(packet);
        }

        /// <summary>Sends a packet to the server via UDP.</summary>
        /// <param name="packet">The packet to send to the sever.</param>
        private static void SendUdpData(Packet packet)
        {
            packet.WriteLength();
            Client.instance.udp.SendData(packet);
        }

        #region Packets
        /// <summary>Lets the server know that the welcome message was received.</summary>
        public static void ClientConnectionReceived()
        {
            using (Packet packet = new Packet((int) Packets.clientConnectionRecived))
            {
                SendTcpData(packet);
            }
        }

        public static void DebugMessage(string message)
        {
            using (Packet packet = new Packet((int) Packets.debugMessage))
            {
                packet.Write(message);
                SendTcpData(packet);
            }
        }
        #endregion
    }
}
