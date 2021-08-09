using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;



public class VideoDecoder : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        // create encoder and decoder
        var encoder = new OpenH264Lib.Encoder("openh264-2.1.1-win32.dll");
        var decoder = new OpenH264Lib.Decoder("openh264-2.1.1-win32.dll");

// setup encoder
        float fps = 10.0f;
        int bps = 5000 * 1000; // target bitrate. 5Mbps.
        float keyFrameInterval = 2.0f; // insert key frame interval. unit is second.
        encoder.Setup(640, 480, bps, fps, keyFrameInterval, (data, length, frameType) =>
        {
            // called when each frame encoded.
            Debug.LogFormat("Encord {0} bytes, frameType:{1}", length, frameType);

            // decode it to Bitmap again...
            var bmp = decoder.Decode(data, length);
            if (bmp != null) Debug.LogFormat(bmp.Size);
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
