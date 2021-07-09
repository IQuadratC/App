using System;
using System.Net;
using System.Net.Sockets;
using Network.V2.Both;
using UnityEngine;

namespace Network.V2.Client
{
    public class UDPClient
    {
        private Client client;

        public UDPClient(Client client)
        {
            this.client = client;
        }
        
         public UdpClient socket; 
         public IPEndPoint endPoint;

         public void Connect()
         {
             socket = new UdpClient(client.port.value);
             endPoint = new IPEndPoint(IPAddress.Parse(client.ip.value), client.port.value);

             socket.Connect(endPoint);
             socket.BeginReceive(ReceiveCallback, null);
         }
         
         private void ReceiveCallback(IAsyncResult result)
         {
             if (client.clientState.value == (int) NetworkState.notConnected) { return; }
                
             try
             {
                 byte[] data = socket.EndReceive(result, ref endPoint);
                 socket.BeginReceive(ReceiveCallback, null);

                 if (data.Length < 4)
                 {
                     client.Disconnect();
                     return;
                 }
                 client.HandleData(data);
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
                     socket.BeginSend(data, length, null, null);
                 }
             }
             catch (Exception ex)
             {
                 Debug.Log($"CLIENT: Error sending data to server via UDP: {ex}");
             }
         }
         
         public void Disconnect()
         {
             endPoint = null;
             socket = null;
         }
    }
}