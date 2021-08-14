using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Net;
using System.IO;
using Utility;

public class WebStream : MonoBehaviour
{
    [HideInInspector]
    public Byte[] JpegData;
    [HideInInspector]
    public string resolution = "480x360";

    private Texture2D texture;
    private Stream stream;
    private WebResponse resp;
    public MeshRenderer frame;

    [SerializeField] private PublicString ip;
    [SerializeField] private PublicBool CamOnline;

    private bool online;
    
    private void Update()
    {
        if (CamOnline.value && !online)
        {
            online = true;
            GetVideo();
        }

        if (!CamOnline.value && online)
        {
            StopStream();
        }
    }

    private void OnApplicationQuit()
    {
        StopStream();
    }

    public void StopStream()
    {
        online = false;
        stream.Close();
        resp.Close();
    }

    public void GetVideo()
    {
        texture = new Texture2D(2, 2);

        string url = "http://" +ip.value+ ":8000/stream.mjpg";

        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

        resp = req.GetResponse();

        stream = resp.GetResponseStream();
        StartCoroutine(GetFrame());
    }

    public IEnumerator GetFrame()
    {
        Byte[] JpegData = new Byte[505536];
        while (true)
        {
            int bytesToRead = FindLength(stream);
            if (bytesToRead == -1)
            {
                yield break;
            }

            int leftToRead = bytesToRead;

            while (leftToRead > 0)
            { 
                leftToRead -= stream.Read(JpegData, bytesToRead - leftToRead, leftToRead);
                yield return null;
            }

            MemoryStream ms = new MemoryStream(JpegData, 0, bytesToRead, false, true);

            texture.LoadImage(ms.GetBuffer());
            frame.material.mainTexture = texture;
            frame.material.color = Color.white;
            stream.ReadByte(); // CR after bytes
            stream.ReadByte(); // LF after bytes
        }
    }

    int FindLength(Stream stream)
    {
        int b;
        string line = "";
        int result = -1;
        bool atEOL = false;
        while ((b = stream.ReadByte()) != -1)
        {
            if (b == 10) continue; // ignore LF char
            if (b == 13)
            { // CR
                if (atEOL)
                {  // two blank lines means end of header
                    stream.ReadByte(); // eat last LF
                    return result;
                }
                if (line.StartsWith("Content-Length:"))
                {
                    result = Convert.ToInt32(line.Substring("Content-Length:".Length).Trim());
                }
                else
                {
                    line = "";
                }
                atEOL = true;
            }
            else
            {
                atEOL = false;
                line += (char)b;
            }
        }
        return -1;
    }
}
