using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using Utility;
using Debug = UnityEngine.Debug;

namespace Network.Client
{
    public class Client : MonoBehaviour
    {
        public static Client instance;
        private const int dataBufferSize = 4096;

        [SerializeField] private PublicString ip;
        [SerializeField] private PublicInt port;
        [SerializeField] public PublicInt clientId;
        
        public Tcp tcp;
        public Udp udp;
        private delegate void PacketHandler(Packet packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        [SerializeField] private PublicEvent connectEvent;
        [SerializeField] private PublicEvent disconnectEvent;
        [SerializeField] private PublicInt clientState;
        
        [SerializeField] private PublicEventString debugEvent;

        [SerializeField] public RenderTexture debugTexture;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("CLIENT: Instance already exists, destroying object!");
                Destroy(this);
            }

            connectEvent.Register(Connect);
            disconnectEvent.Register(Disconnect);
            clientState.value = (int) NetworkState.notConnected;
            debugEvent.Register(ClientSend.DebugMessage);
        }
    
        private void OnApplicationQuit()
        {
            Disconnect(); // Disconnect when the game is closed
        }

        /// <summary>Attempts to connect to the server.</summary>
        public void Connect()
        {
            if (clientState.value == (int) NetworkState.connected) { return; }
            
            Debug.Log("CLIENT: Trying to connect...");
            
            tcp = new Tcp();
            udp = new Udp();
        
            InitializeClientData();
            
            tcp.Connect(); // Connect tcp, udp gets connected once tcp is done
        }

        public class Tcp
        {
            public TcpClient socket;

            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            /// <summary>Attempts to connect to the server via TCP.</summary>
            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };

                receiveBuffer = new byte[dataBufferSize];
                socket.BeginConnect(instance.ip.value, instance.port.value, ConnectCallback, socket);
                
                Threader.RunAsync(() =>
                {
                    Thread.Sleep(2000);
                    if (instance.clientState.value == (int) NetworkState.notConnected)
                    {
                        Threader.RunOnMainThread(() =>
                        {
                            Debug.Log("CLIENT: Connect Timeout");
                        });
                        instance.tcp.socket.Close();
                    }
                });
            }

            /// <summary>Initializes the newly connected client's TCP-related info.</summary>
            private void ConnectCallback(IAsyncResult result)
            {
                socket.EndConnect(result);

                if (!socket.Connected)
                {
                    instance.clientState.value = (int) NetworkState.notConnected;
                    return;
                }
                instance.clientState.value = (int) NetworkState.connected;

                stream = socket.GetStream();

                receivedData = new Packet();

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }

            /// <summary>Sends data to the client via TCP.</summary>
            /// <param name="packet">The packet to send.</param>
            public void SendData(Packet packet)
            {
                if (instance.clientState.value == (int) NetworkState.notConnected) { return; }
                
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null); // Send data to server
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log($"CLIENT:: Error sending data to server via TCP: {ex}");
                }
            }

            /// <summary>Reads incoming data from the stream.</summary>
            private void ReceiveCallback(IAsyncResult result)
            {
                if (instance.clientState.value == (int) NetworkState.notConnected) { return; }
                
                try
                {
                    int byteLength = stream.EndRead(result);
                    if (byteLength <= 0)
                    {
                        instance.Disconnect();
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(receiveBuffer, data, byteLength);

                    receivedData.Reset(HandleData(data)); // Reset receivedData if all data was handled
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch
                {
                    Disconnect();
                }
            }

            /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
            /// <param name="data">The recieved data.</param>
            private bool HandleData(byte[] data)
            {
                UInt16 packetLength = 0;

                receivedData.SetBytes(data);

                if (receivedData.UnreadLength() >= 2)
                {
                    // If client's received data contains a packet
                    packetLength = receivedData.ReadUInt16();
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
                            int packetId = packet.ReadUInt16();
                            packetHandlers[packetId](packet); // Call appropriate method to handle the packet
                        }
                    });

                    packetLength = 0; // Reset packet length
                    if (receivedData.UnreadLength() >= 2)
                    {
                        // If client's received data contains another packet
                        packetLength = receivedData.ReadUInt16();
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

            /// <summary>Disconnects from the server and cleans up the TCP connection.</summary>
            private void Disconnect()
            {
                instance.Disconnect();

                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        public class Udp
        {
            public UdpClient socket;
            public IPEndPoint endPoint;

            public Udp()
            {
                endPoint = new IPEndPoint(IPAddress.Parse(instance.ip.value), instance.port.value);
            }

            /// <summary>Attempts to connect to the server via UDP.</summary>
            /// <param name="localPort">The port number to bind the UDP socket to.</param>
            public void Connect(int localPort)
            {
                socket = new UdpClient(localPort);

                socket.Connect(endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                using (Packet packet = new Packet())
                {
                    SendData(packet);
                }
            }

            /// <summary>Sends data to the client via UDP.</summary>
            /// <param name="packet">The packet to send.</param>
            public void SendData(Packet packet)
            {
                if (instance.clientState.value == (int) NetworkState.notConnected) { return; }
                
                try
                {
                    packet.InsertInt(instance.clientId.value); // Insert the client's ID at the start of the packet
                    if (socket != null)
                    {
                        socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log($"CLIENT: Error sending data to server via UDP: {ex}");
                }
            }

            /// <summary>Receives incoming UDP data.</summary>
            private void ReceiveCallback(IAsyncResult result)
            {
                if (instance.clientState.value == (int) NetworkState.notConnected) { return; }
                
                try
                {
                    byte[] data = socket.EndReceive(result, ref endPoint);
                    socket.BeginReceive(ReceiveCallback, null);

                    if (data.Length < 2)
                    {
                        instance.Disconnect();
                        return;
                    }

                    HandleData(data);
                }
                catch
                {
                    Disconnect();
                }
            }

            /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
            /// <param name="data">The recieved data.</param>
            private static void HandleData(byte[] data)
            {
                using (Packet packet = new Packet(data))
                {
                    UInt16 packetLength = packet.ReadUInt16();
                    data = packet.ReadBytes(packetLength);
                }

                Threader.RunOnMainThread(() =>
                {
                    using (Packet packet = new Packet(data))
                    {
                        int packetId = packet.ReadUInt16();
                        packetHandlers[packetId](packet); // Call appropriate method to handle the packet
                    }
                });
            }

            /// <summary>Disconnects from the server and cleans up the UDP connection.</summary>
            private void Disconnect()
            {
                instance.Disconnect();

                endPoint = null;
                socket = null;
            }
        }
    
        /// <summary>Initializes all necessary client data.</summary>
        private void InitializeClientData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)Packets.serverConnection, ClientHandle.ServerConnection },
                { (int)Packets.debugMessage, ClientHandle.DebugMessage },
                { (int)Packets.debugImage, ClientHandle.DebugImage },
            };
        }

        /// <summary>Disconnects from the server and stops all network traffic.</summary>
        public void Disconnect()
        {
            if (clientState.value == (int) NetworkState.notConnected) return;

            clientState.value = (int) NetworkState.notConnected;
            tcp.socket.Close();
            if (udp.socket != null)
            {
                udp.socket.Close();
            }
            
            Debug.Log("CLIENT: Disconnected from server.");
            
        }
    }
}