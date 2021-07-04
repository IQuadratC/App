using System;
using System.Net;
using System.Net.Sockets;
using SharedFiles.Utility;
using UnityEngine;
using Utility;

namespace Network.Server
{
    public class Client
    {
        private const int DataBufferSize = 4096;

        public readonly int id;
        public readonly Tcp tcp;
        public readonly Udp udp;

        public Client(int clientId)
        {
            id = clientId;
            tcp = new Tcp(id);
            udp = new Udp(id);
        }

        public class Tcp
        {
            public TcpClient socket;

            private readonly int id;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public Tcp(int id)
            {
                this.id = id;
            }

            /// <summary>Initializes the newly connected client's TCP-related info.</summary>
            /// <param name="clientSocket">The TcpClient instance of the newly connected client.</param>
            public void Connect(TcpClient clientSocket)
            {
                socket = clientSocket;
                socket.ReceiveBufferSize = DataBufferSize;
                socket.SendBufferSize = DataBufferSize;

                stream = socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[DataBufferSize];

                stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);

                ServerSend.ServerConnection(id);
            }

            /// <summary>Sends data to the client via TCP.</summary>
            /// <param name="packet">The packet to send.</param>
            public void SendData(Packet packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null); // Send data to appropriate client
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log($"SERVER: Error sending data to player {id} via TCP: {ex}");
                }
            }

            /// <summary>Reads incoming data from the stream.</summary>
            private void ReceiveCallback(IAsyncResult result)
            {
                if (stream == null) { return; }
                
                try
                {
                    int byteLength = stream.EndRead(result);
                    if (byteLength <= 0)
                    {
                        Server.instance.clients[id].Disconnect();
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(receiveBuffer, data, byteLength);

                    receivedData.Reset(HandleData(data)); // Reset receivedData if all data was handled
                    stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
                }
                catch (Exception ex)
                {
                    Debug.Log($"SERVER: Error receiving TCP data: {ex}");
                    Server.instance.clients[id].Disconnect();
                }
            }

            /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
            /// <param name="data">The recieved data.</param>
            private bool HandleData(byte[] data)
            {
                int packetLength = 0;

                receivedData.SetBytes(data);

                if (receivedData.UnreadLength() >= 4)
                {
                    // If client's received data contains a packet
                    packetLength = receivedData.ReadInt();
                    if (packetLength <= 0)
                    {
                        // If packet contains no data
                        return true; // Reset receivedData instance to allow it to be reused
                    }
                }

                while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
                {
                    // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
                    byte[] packetBytes = receivedData.ReadBytes(packetLength);
                    Threader.RunOnMainThread(() =>
                    {
                        using (Packet packet = new Packet(packetBytes))
                        {
                            int packetId = packet.ReadInt();
                            Server.instance.packetHandlers[packetId](id, packet); // Call appropriate method to handle the packet
                        }
                    });

                    packetLength = 0; // Reset packet length
                    if (receivedData.UnreadLength() >= 4)
                    {
                        // If client's received data contains another packet
                        packetLength = receivedData.ReadInt();
                        if (packetLength <= 0)
                        {
                            // If packet contains no data
                            return true; // Reset receivedData instance to allow it to be reused
                        }
                    }
                }

                if (packetLength <= 1)
                {
                    return true; // Reset receivedData instance to allow it to be reused
                }

                return false;
            }

            /// <summary>Closes and cleans up the TCP connection.</summary>
            public void Disconnect()
            {
                socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        public class Udp
        {
            public IPEndPoint endPoint;

            private readonly int id;

            public Udp(int id)
            {
                this.id = id;
            }

            /// <summary>Initializes the newly connected client's UDP-related info.</summary>
            /// <param name="endPoint">The IPEndPoint instance of the newly connected client.</param>
            public void Connect(IPEndPoint endPoint)
            {
                this.endPoint = endPoint;
            }

            /// <summary>Sends data to the client via UDP.</summary>
            /// <param name="packet">The packet to send.</param>
            public void SendData(Packet packet)
            {
                Server.instance.SendUdpData(endPoint, packet);
            }

            /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
            /// <param name="packetData">The packet containing the recieved data.</param>
            public void HandleData(Packet packetData)
            {
                int packetLength = packetData.ReadInt();
                byte[] packetBytes = packetData.ReadBytes(packetLength);

                Threader.RunOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        Server.instance.packetHandlers[packetId](id, packet); // Call appropriate method to handle the packet
                    }
                });
            }

            /// <summary>Cleans up the UDP connection.</summary>
            public void Disconnect()
            {
                endPoint = null;
            }
        }

        /// <summary>Disconnects the client and stops all network traffic.</summary>
        public void Disconnect()
        {
            Debug.Log($"SERVER: {tcp.socket.Client.RemoteEndPoint} has disconnected.");
            
            tcp.Disconnect();
            udp.Disconnect();
        }
    }
}


