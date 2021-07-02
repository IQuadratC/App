using Player;
using Player.Trooper;
using SharedFiles.Utility;
using UI;
using UI.Panles;
using UnityEngine;

namespace Server
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
        public static void ServerConnectionReceived()
        {
            using (Packet packet = new Packet((int)ClientPackets.gameEnterRequest))
            {
                packet.Write(Client.instance.myId);
                packet.Write(UIManager.gameSettings.playerName);
                packet.Write(StartPanel.instance.currentSelectedServer.password);

                SendTcpData(packet);
            }
        }

        /// <summary>Sends player input to the server.</summary>
        public static void TrooperTransformUpdate(Trooper trooper)
        {
            using (Packet packet = new Packet((int)ClientPackets.trooperTransformUpdate))
            {
                packet.Write(trooper.transform.position);
                packet.Write(trooper.transform.rotation);
                packet.Write(trooper.velocity);
                packet.Write(trooper.grounded);

                SendUdpData(packet);
            }
        }
        
        public static void TrooperGrappleUpdate(Trooper trooper)
        {
            using (Packet packet = new Packet((int)ClientPackets.trooperGrappleUpdate))
            {
                packet.Write(trooper.isGrappling);
                packet.Write(trooper.grappleObjectId);
                packet.Write(trooper.grapplePoint);
                packet.Write(trooper.maxDistanceFromGrapple);

                SendUdpData(packet);
            }
        }
        #endregion
    }
}
