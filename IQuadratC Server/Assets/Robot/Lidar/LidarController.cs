using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Utility;

public class LidarController : MonoBehaviour
{
    [SerializeField] private PublicEventBool lidarModeEvent;
    [SerializeField] private PublicBool lidarMode;
    
    [SerializeField] private float minLength = 0.1f;
    [SerializeField] private float maxLength = 8.0f;

    void Start()
    {
        lidarModeEvent.Register((value) =>
        {
            if (value && !lidarMode)
            {
                lidarMode.value = true;
            }
            else if (!value && lidarMode)
            {
                lidarMode.value = false;
            }
        });
    }

    private void Update()
    {
        if (lidarMode.value)
        {
            scan();
        }
    }

    [SerializeField] private PublicFloat2Array dataArrayPolar;
    [SerializeField] private PublicFloat2Array dataArray;

    [SerializeField] private bool showLines;
    
    private void OnDrawGizmos()
    {
        if (showLines)
        {
            foreach (float2 data in dataArray.value)
            {
                float3 robot = new float3(transform.position.x, 10, transform.position.z);
                float3 point = new float3(data.x, 10, data.y);
                Gizmos.DrawLine(robot + math.normalize(point - robot) * minLength, point);
            }
        }
    }

    private void scan()
    {
        List<float2> dataListPolar = new List<float2>();
        List<float2> dataList = new List<float2>();
        
        int layerMask = 1 << 8;
        for (float i = 0; i < 365; i++)
        {
            var direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * i), 0, Mathf.Cos(Mathf.Deg2Rad * i));
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(transform.position.x, 10, transform.position.z) + direction * minLength, direction, out hit, maxLength - minLength))
            {
                dataListPolar.Add(new float2(i, hit.distance));
                dataList.Add(new float2(hit.point.x, hit.point.z));
            }
        }

        dataArrayPolar.value = dataListPolar.ToArray();
        dataArray.value = dataList.ToArray();
    }
}
