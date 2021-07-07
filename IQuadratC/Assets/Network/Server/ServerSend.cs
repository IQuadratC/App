namespace Network.Server
{
    public static class ServerSend
    {
        /// <summary>Sends a packet to a client via TCP.</summary>
        /// <param name="toClient">The client to send the packet the packet to.</param>
        /// <param name="packet">The packet to send to the client.</param>
        private static void SendTcpData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.instance.clients[toClient].tcp.SendData(packet);
        }

        /// <summary>Sends a packet to a client via UDP.</summary>
        /// <param name="toClient">The client to send the packet the packet to.</param>
        /// <param name="packet">The packet to send to the client.</param>
        private static void SendUdpData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.instance.clients[toClient].udp.SendData(packet);
        }

        /// <summary>Sends a packet to all clients via TCP.</summary>
        /// <param name="packet">The packet to send.</param>
        private static void SendTcpDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.instance.maxClients; i++)
            {
                Server.instance.clients[i].tcp.SendData(packet);
            }
        }
        /// <summary>Sends a packet to all clients except one via TCP.</summary>
        /// <param name="exceptClient">The client to NOT send the data to.</param>
        /// <param name="packet">The packet to send.</param>
        private static void SendTcpDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.instance.maxClients; i++)
            {
                if (i != exceptClient)
                {
                    Server.instance.clients[i].tcp.SendData(packet);
                }
            }
        }

        /// <summary>Sends a packet to all clients via UDP.</summary>
        /// <param name="packet">The packet to send.</param>
        private static void SendUdpDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.instance.maxClients; i++)
            {
                Server.instance.clients[i].udp.SendData(packet);
            }
        }
        /// <summary>Sends a packet to all clients except one via UDP.</summary>
        /// <param name="exceptClient">The client to NOT send the data to.</param>
        /// <param name="packet">The packet to send.</param>
        private static void SendUdpDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.instance.maxClients; i++)
            {
                if (i != exceptClient)
                {
                    Server.instance.clients[i].udp.SendData(packet);
                }
            }
        }

        #region Packets

        /// <summary>Sends a welcome message to the given client.</summary>
        public static void ServerConnection(int toClient)
        {
            using (Packet packet = new Packet((int) Packets.serverConnection))
            {
                packet.Write(toClient);
                SendTcpData(toClient, packet);
            }
        }
        public static void DebugMessage(string message)
        {
            using (Packet packet = new Packet((int) Packets.debugMessage))
            {
                packet.Write(message);
                SendTcpDataToAll(packet);
            }
        }
        #endregion
    }
}
