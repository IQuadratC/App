using System;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace Network.V1
{
    /// <summary>Packet enum</summary>
    public enum Packets
    {
        serverConnection = 1,
        clientConnectionRecived = 2,
        debugMessage = 3,
        debugImage = 4,

    }

    public enum NetworkState
    {
        notConnected = 1,
        connected = 2,
    }
    
    
    public class Packet : IDisposable
    {
        private List<byte> buffer;
        private byte[] readableBuffer;
        private int readPos;

        /// <summary>Creates a new empty packet (without an ID).</summary>
        public Packet()
        {
            buffer = new List<byte>(); // Initialize buffer
            readPos = 0; // Set readPos to 0
        }

        /// <summary>Creates a new packet with a given ID. Used for sending.</summary>
        /// <param name="id">The packet ID.</param>
        public Packet(byte id)
        {
            buffer = new List<byte>(); // Initialize buffer
            readPos = 0; // Set readPos to 0

            Write(id); // Write packet id to the buffer
        }

        /// <summary>Creates a packet from which data can be read. Used for receiving.</summary>
        /// <param name="data">The bytes to add to the packet.</param>
        public Packet(byte[] data)
        {
            buffer = new List<byte>(); // Initialize buffer
            readPos = 0; // Set readPos to 0

            SetBytes(data);
        }

        #region Functions
        /// <summary>Sets the packet's content and prepares it to be read.</summary>
        /// <param name="data">The bytes to add to the packet.</param>
        public void SetBytes(byte[] data)
        {
            Write(data);
            readableBuffer = buffer.ToArray();
        }
        public void InsertByte(byte value)
        {
            buffer.Insert(0, value); // Insert the int at the start of the buffer
        }
        public void InsertUInt16(UInt16 value)
        {
            buffer.InsertRange(0, BitConverter.GetBytes(value)); // Insert the int at the start of the buffer
        }
        public void InsertInt(int value)
        {
            buffer.InsertRange(0, BitConverter.GetBytes(value)); // Insert the int at the start of the buffer
        }
        public void WriteLength()
        {
            InsertInt(buffer.Count);
        }

        /// <summary>Gets the packet's content in array form.</summary>
        public byte[] ToArray()
        {
            readableBuffer = buffer.ToArray();
            return readableBuffer;
        }

        /// <summary>Gets the length of the packet's content.</summary>
        public int Length()
        {
            return buffer.Count; // Return the length of buffer
        }

        /// <summary>Gets the length of the unread data contained in the packet.</summary>
        public int UnreadLength()
        {
            return Length() - readPos; // Return the remaining length (unread)
        }

        /// <summary>Resets the packet instance to allow it to be reused.</summary>
        /// <param name="shouldReset">Whether or not to reset the packet.</param>
        public void Reset(bool shouldReset = true)
        {
            if (shouldReset)
            {
                buffer.Clear(); // Clear buffer
                readableBuffer = null;
                readPos = 0; // Reset readPos
            }
            else
            {
                readPos -= 4; // "Unread" the last read int
            }
        }
        #endregion

        #region Write Data
        public void Write(byte value)
        {
            buffer.Add(value);
        }
        public void Write(byte[] value)
        {
            buffer.AddRange(value);
        }
        public void Write(short value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }
        
        public void Write(UInt16 value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(int value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(long value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(float value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(bool value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(string value)
        {
            Write(value.Length); // Add the length of the string to the packet
            buffer.AddRange(Encoding.ASCII.GetBytes(value)); // Add the string itself
        }
        public void Write(int2 value)
        {
            Write(value.x);
            Write(value.y);
        }
        public void Write(int3 value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
        }
        public void Write(float2 value)
        {
            Write(value.x);
            Write(value.y);
        }
        public void Write(float3 value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
        }
        public void Write(float4 value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
            Write(value.w);
        }

        public void Write(Texture2D value)
        {
            int2 size = (int2) (float2) value.Size();
            Write(size);
            
            Color[] pixels = value.GetPixels(0,0,size.x, size.y);
            foreach (Color pixel in pixels)
            {
                Write(new float4(pixel.r, pixel.g, pixel.b, pixel.a));
            }
        }
        
        public void Write(Quaternion value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
            Write(value.w);
        }
        #endregion

        #region Read Data
        /// <summary>Reads a byte from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public byte ReadByte(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                byte value = readableBuffer[readPos]; // Get the byte at readPos' position
                if (moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 1; // Increase readPos by 1
                }
                return value; // Return the byte
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }

        /// <summary>Reads an array of bytes from the packet.</summary>
        /// <param name="length">The length of the byte array.</param>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public byte[] ReadBytes(int length, bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                byte[] value = buffer.GetRange(readPos, length).ToArray(); // Get the bytes at readPos' position with a range of _length
                if (moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += length; // Increase readPos by _length
                }
                return value; // Return the bytes
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }

        /// <summary>Reads a short from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public short ReadShort(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                short value = BitConverter.ToInt16(readableBuffer, readPos); // Convert the bytes to a short
                if (moveReadPos)
                {
                    // If _moveReadPos is true and there are unread bytes
                    readPos += 2; // Increase readPos by 2
                }
                return value; // Return the short
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }
        
        /// <summary>Reads a short from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public UInt16 ReadUInt16(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                UInt16 value = BitConverter.ToUInt16(readableBuffer, readPos); // Convert the bytes to a short
                if (moveReadPos)
                {
                    // If _moveReadPos is true and there are unread bytes
                    readPos += 2; // Increase readPos by 2
                }
                return value; // Return the short
            }
            else
            {
                throw new Exception("Could not read value of type 'UInt16'!");
            }
        }

        /// <summary>Reads an int from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public int ReadInt(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                int value = BitConverter.ToInt32(readableBuffer, readPos); // Convert the bytes to an int
                if (moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 4; // Increase readPos by 4
                }
                return value; // Return the int
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }

        /// <summary>Reads a long from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public long ReadLong(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                long value = BitConverter.ToInt64(readableBuffer, readPos); // Convert the bytes to a long
                if (moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 8; // Increase readPos by 8
                }
                return value; // Return the long
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }

        /// <summary>Reads a float from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public float ReadFloat(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                float value = BitConverter.ToSingle(readableBuffer, readPos); // Convert the bytes to a float
                if (moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 4; // Increase readPos by 4
                }
                return value; // Return the float
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }

        /// <summary>Reads a bool from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public bool ReadBool(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                bool value = BitConverter.ToBoolean(readableBuffer, readPos); // Convert the bytes to a bool
                if (moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 1; // Increase readPos by 1
                }
                return value; // Return the bool
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }

        /// <summary>Reads a string from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public string ReadString(bool moveReadPos = true)
        {
            try
            {
                int length = ReadInt(); // Get the length of the string
                string value = Encoding.ASCII.GetString(readableBuffer, readPos, length); // Convert the bytes to a string
                if (moveReadPos && value.Length > 0)
                {
                    // If _moveReadPos is true string is not empty
                    readPos += length; // Increase readPos by the length of the string
                }
                return value; // Return the string
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }
        public int2 ReadInt2(bool moveReadPos = true)
        {
            return new int2(ReadInt(moveReadPos), ReadInt(moveReadPos));
        }
        public int3 ReadInt3(bool moveReadPos = true)
        {
            return new int3(ReadInt(moveReadPos), ReadInt(moveReadPos), ReadInt(moveReadPos));
        }
        public float2 ReadFloat2(bool moveReadPos = true)
        {
            return new float2(ReadFloat(moveReadPos), ReadFloat(moveReadPos));
        }
        public float3 ReadFloat3(bool moveReadPos = true)
        {
            return new float3(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
        }
        public float4 ReadFloat4(bool moveReadPos = true)
        {
            return new float4(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
        }
        public Texture2D ReadTexture2D(bool moveReadPos = true)
        {
            int2 size = ReadInt2();
            Texture2D texture = new Texture2D(size.x, size.y);

            Color[] pixels = new Color[size.x * size.y];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
            }
            texture.SetPixels(pixels);

            return texture;
        }

        
        public Quaternion ReadQuaternion(bool moveReadPos = true)
        {
            return new Quaternion(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
        }
        #endregion

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    buffer = null;
                    readableBuffer = null;
                    readPos = 0;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}