using System;
using System.Text;
using UnityEngine;
using Utility;

namespace Network
{
    enum TCPMessageIds
    {
        debugText = 1
    }
    
    public class TCPMessage
    {
        public static void ParseMessage(byte[] data)
        {
            int id = BitConverter.ToUInt16(data, 0);
            int length = BitConverter.ToUInt16(data, 2);

            if (data.Length != length + 4)
            {

            }

            switch ((TCPMessageIds) id)
            {
                case TCPMessageIds.debugText:
                    String text = Encoding.UTF8.GetString(data, 4, length);
                    Threader.RunOnMainThread(() =>
                    {
                        Debug.Log("Debug: " + text);
                    });
                    break;
            }
        }
        
        public static byte[] CreateMessage(UInt16 id, byte[] data)
        {
            byte[] message = new byte[data.Length + 4];
            BitConverter.GetBytes(id).CopyTo(message, 0);
            BitConverter.GetBytes((UInt16) data.Length).CopyTo(message, 2);
            data.CopyTo(message, 4);

            return message;
        }
    }
    
    
}