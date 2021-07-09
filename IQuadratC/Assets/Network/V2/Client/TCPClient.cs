using System;
using System.Net.Sockets;
using System.Threading;
using Network.V2.Both;
using UnityEngine;
using Utility;

namespace Network.V2.Client
{
    public class TCPClient
    {
        private Client client;

        public TCPClient(Client client)
        {
            this.client = client;
        }
        
        public TcpClient socket;
        private NetworkStream stream;
        private byte[] receiveBuffer;
        
         public void Connect()
         {
             socket = new TcpClient
            {
                ReceiveBufferSize = State.BufferSize,
                SendBufferSize = State.BufferSize
            };

            receiveBuffer = new byte[State.BufferSize];
            socket.BeginConnect(client.ip.value, client.port.value, ConnectCallback, socket);
            
            Threader.RunAsync(() =>
            {
                Thread.Sleep(2000);
                if (client.clientState.value == (int) NetworkState.notConnected)
                {
                    Threader.RunOnMainThread(() =>
                    {
                        Debug.Log("CLIENT: Connect Timeout");
                    });
                    socket.Close();
                }
            });
        }
         
         private void ConnectCallback(IAsyncResult result)
         {
             socket.EndConnect(result);

             if (!socket.Connected)
             {
                 client.clientState.value = (int) NetworkState.notConnected;
                 return;
             }
             client.clientState.value = (int) NetworkState.connected;

             stream = socket.GetStream();
             
             stream.BeginRead(receiveBuffer, 0, State.BufferSize, ReceiveCallback, null);
         }
         
          private void ReceiveCallback(IAsyncResult result)
         {
             if (client.clientState.value == (int) NetworkState.notConnected) { return; }
             
             try
             {
                 int byteLength = stream.EndRead(result);
                 if (byteLength < 4)
                 {
                     client.Disconnect();
                     return;
                 }

                 byte[] data = new byte[byteLength];
                 Array.Copy(receiveBuffer, data, byteLength);

                 client.HandleData(data);
                 stream.BeginRead(receiveBuffer, 0, State.BufferSize, ReceiveCallback, null);
             }
             catch
             {
                 client.Disconnect();
             }
         }
         
         public void SendData(byte[] data, int length)
         {
             if (client.clientState.value == (int) NetworkState.notConnected) { return; }
             try
             {
                 if (socket != null)
                 {
                     stream.BeginWrite(data, 0, length, null, null);
                 }
             }
             catch (Exception ex)
             {
                 Debug.Log($"CLIENT:: Error sending data to server via TCP: {ex}");
             }
         }
         
         public void Disconnect()
         {
             stream = null;
             receiveBuffer = null;
             socket = null;
         }
    }
}