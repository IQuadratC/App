using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Utility;

public class SLAMController : MonoBehaviour
{
    [SerializeField] private PublicFloat2Array lidarDataPolar;
    [SerializeField] private PublicFloat2Array lidarData;
    [SerializeField] private PublicFloat3 pos;
    [SerializeField] private PublicByteArray grid;
    
    [SerializeField] private PublicInt minLidarDistance;
    [SerializeField] private PublicInt maxLidarDistance;

    private int convertIndex(float x, float y)
    {
        return (int) ((x + gridSize) / gridIntervall) * (gridSize / gridIntervall) * 2 + (int) (y + gridSize) / gridIntervall; 
    }

    [SerializeField] private int gridSize = 1000;
    [SerializeField] private int gridIntervall = 10; 
    
    void Start()
    {
        grid.value = new byte[gridSize * gridSize * 4 / (gridIntervall * gridIntervall)];
    }

    private void OnDrawGizmos()
    {
        if (grid != null && grid.value.Length > 0)
        {
            for (int x = -gridSize; x < gridSize; x += gridIntervall)
            {
                for (int y = -gridSize; y < gridSize; y += gridIntervall)
                {
                    byte gridCell = grid.value[convertIndex(x, y)];
                    if (gridCell == 0)
                    {
                        Gizmos.color = Color.grey;
                    }
                    else if (gridCell == 1)
                    {
                        Gizmos.color = Color.green;
                    }
                    else if (gridCell == 2)
                    {
                        Gizmos.color = Color.red;
                    }
                    Gizmos.DrawCube(new Vector3(x + gridIntervall / 2, y + gridIntervall / 2, -20), Vector3.one * 2);
                }
            }
        }
    }

    void Update()
    {
        int2 middleCell = (int2) (pos.value.xy / gridIntervall) * gridIntervall;
        for (int x = middleCell.x - maxLidarDistance.value; x < pos.value.x + maxLidarDistance.value; x += gridIntervall)
        {
            for (int y = middleCell.y - maxLidarDistance.value; y < pos.value.y + maxLidarDistance.value; y += gridIntervall)
            {
                float2 polarPoint = mathAdditions.Cartisian2PolarDegree(new float2(x, y) - pos.value.xy);

                while (polarPoint.x < 0)
                {
                    polarPoint.x += 360;
                }
                
                while (polarPoint.x >= 360)
                {
                    polarPoint.x -= 360;
                }
                
                if (lidarDataPolar.value[(int) polarPoint.x].y > polarPoint.y)
                {
                    int index = convertIndex(x, y);
                    if (grid.value[index] == 0)
                    {
                        grid.value[index] = 1;
                    }
                }
            }
        }
        
        foreach (float2 point in lidarData.value)
        {
            if (math.distance(point, pos.value.xy) < minLidarDistance.value)
            {
               continue;
            }
            int index = convertIndex(point.x, point.y);
            grid.value[index] = 2;
        }
    }
}
