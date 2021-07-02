using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Network;
using UnityEngine;
using Utility;

public class TCPClient : MonoBehaviour
{
    [SerializeField] private PublicString ip;
    [SerializeField] private PublicInt tcpPort;

    private Socket s;
    private IPEndPoint ipe;
    [SerializeField] private NetworkState state;
    
    private const int bufferSize = 1024;
    private byte[] buffer;

    [SerializeField] private PublicEvent connectClientEvent;
    [SerializeField] private PublicEvent disconnectClientEvent;
    
    private void Start()
    {
        state = NetworkState.notConnected;
        
        buffer = new byte[bufferSize];

        connectClientEvent.Register(Connect);
        disconnectClientEvent.Register(Disconnect);
    }

    private void Connect()
    {
        if (state != NetworkState.notConnected) {return;}
        
        s = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        
        ipe = new IPEndPoint(IPAddress.Parse(ip.value),tcpPort.value);
        
        s.BeginConnect(ipe, ConnectCallback, null);
        
        state = NetworkState.connecting;
    }
    private void ConnectCallback(IAsyncResult ar)
    {
        try {
            s.EndConnect(ar);
            state = NetworkState.idle;
  
            Debug.LogFormat("TCP Client: Connected to {0}", s.RemoteEndPoint.ToString());

        } catch (Exception e) {  
            Threader.RunOnMainThread(() =>
            {
                Debug.Log(e.ToString());  
            });
        }  
    }
    
    private void Receive() {  
        try {
            s.BeginReceive(buffer, 0, bufferSize, 0, ReceiveCallback, state);
            
            state = NetworkState.reciving;
            
        } catch (Exception e) {  
            Debug.Log(e.ToString());  
        }  
    }
    private void ReceiveCallback( IAsyncResult ar ) {  
        try {
            int bytesRead = s.EndReceive(ar);  
            if (bytesRead > 0) {
                
                s.BeginReceive(buffer,0,bufferSize,0, ReceiveCallback, state);  
                state = NetworkState.reciving;
                
            } else {  
                state = NetworkState.idle;

                ParseMessage(buffer);
                buffer = new byte[0];
            }  
        } catch (Exception e) {  
            Debug.Log(e.ToString());  
        }  
    }
    public void ParseMessage(byte[] data)
    {
        int id = BitConverter.ToUInt16(data, 0);
        int length = BitConverter.ToUInt16(data, 2);

        if (data.Length != length + 4)
        {
            
        }

        switch ((MessageId) id)
        {
            case MessageId.debugText:
                String text = Encoding.UTF8.GetString(data, 4, length);
                Threader.RunOnMainThread(() =>
                {
                    Debug.Log("Debug: " + text);
                });
                break;
            case MessageId.serverDisconnect:
                s.Close();
                state = NetworkState.notConnected;
                Threader.RunOnMainThread(() =>
                {
                    Debug.Log("TCP Client: Disconnected.");  
                });
                break;
        }
    }
    
    
    public void SendMessage(MessageId id, string data)
    {
        SendMessage(id, Encoding.UTF8.GetBytes(data));
    }
    public void SendMessage(MessageId id, byte[] data)
    {
        data ??= new byte[0];
            
        byte[] message = new byte[data.Length + 4];
        BitConverter.GetBytes((UInt16) id).CopyTo(message, 0);
        BitConverter.GetBytes((UInt16) data.Length).CopyTo(message, 2);
        data.CopyTo(message, 4);
        
        Send(message);
    }
    private void Send(byte[] data) {
        
        s.BeginSend(data, 0, data.Length, SocketFlags.None,  
            SendCallback, s);  
        
        state = NetworkState.sending;
    }
    private void SendCallback(IAsyncResult ar) {  
        try {
            
            int bytesSent = s.EndSend(ar);  
            state = NetworkState.idle;
            
            Debug.LogFormat("TCP Client: Sent {0} bytes to server.", bytesSent);
            
        } catch (Exception e) {  
            Debug.Log(e.ToString());  
        }  
    }
    
    private void Disconnect()
    {
        SendMessage(MessageId.clientDisconnect, "");
        state = NetworkState.disconnecting;
        Threader.RunOnMainThread(() =>
        {
            Debug.Log("TCP Client: Disconnecting...");  
        });
    }
}


