using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Utility;

public class LidarController : MonoBehaviour
{
    [SerializeField] private PublicEventInt lidarModeEvent;
    [SerializeField] private PublicInt lidarMode;
    
    [SerializeField] private PublicInt minLidarDistance;
    [SerializeField] private PublicInt maxLidarDistance;

    void Start()
    {
        lidarModeEvent.Register((value) =>
        {
            lidarMode.value = value;
        });
    }

    [SerializeField] private PublicFloat3 pos;

    private void Update()
    {
        pos.value = new float3(transform.position.x, transform.position.y, transform.rotation.eulerAngles.z);
        if (lidarMode.value == 1)
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
            Gizmos.color = new Color(1, 1, 1, 0.1f);
            foreach (float2 data in dataArray.value)
            {
                float3 robot = new float3(transform.position.x, transform.position.y, 0);
                float3 point = new float3(data.xy, 0);
                Gizmos.DrawLine(robot + math.normalize(point - robot) * minLidarDistance.value, point);
            }
        }
    }

    private void scan()
    {
        dataArrayPolar.value = new float2[360];
        dataArray.value = new float2[360];
        
        int layerMask = 1 << 8;
        for (int i = 0; i < 360; i++)
        {
            dataArrayPolar.value[i] = new float2(i, 0);
            dataArray.value[i] = new float2(0, 0);

            float2 direction = mathAdditions.Polar2CartisianDegree(new float2(i, 1));
            RaycastHit hit;
            if (Physics.Raycast(
                new Vector3(transform.position.x, transform.position.y, -10) + new Vector3(direction.x, direction.y, 0) * minLidarDistance.value, 
                new Vector3(direction.x, direction.y, 0), out hit, 
                maxLidarDistance.value - minLidarDistance.value))
            {
                dataArrayPolar.value[i].y = hit.distance + minLidarDistance.value;
            }
            else
            {
                //dataArrayPolar.value[i].y = maxLidarDistance.value;
            }
            dataArray.value[i] = mathAdditions.Polar2CartisianDegree(new float2(i, dataArrayPolar.value[i].y)) + pos.value.xy;
        }
    }
}
