using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Utility;

public class SLAMController : MonoBehaviour
{
    [SerializeField] private PublicFloat2Array liveData;
    [SerializeField] private PublicFloat3 pos;
    [SerializeField] private PublicByteArray grid;

    private int convertIndex(float x, float y)
    {
        return (int) ((x + gridSize) / gridIntervall) * (gridSize / gridIntervall) * 2 + (int) (y + gridSize) / gridIntervall; 
    }

    [SerializeField] private int gridSize = 1000;
    [SerializeField] private int gridIntervall = 100; 
    
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
                    Gizmos.DrawCube(new Vector3(x + gridIntervall / 2, 20, y + gridIntervall / 2), Vector3.one * 2);
                }
            }
        }
    }

    void Update()
    {
        foreach (float2 point in liveData.value)
        {
            castRay(pos.value.xy, point);
            
            int index = convertIndex(point.x, point.y);
            grid.value[index] = 2;
        }
    }

    void castRay(float2 start, float2 end)
    {
        float2 startPos = start / gridIntervall;
        float2 dir = math.normalize(end - start);
        float2 rayStepSize = new float2(
            math.sqrt(1 + (dir.y / dir.x) * (dir.y / dir.x)),
            math.sqrt(1 + (dir.x / dir.y) * (dir.x / dir.y)));
        
        int2 currentCell = (int2) (start / new float2(gridIntervall));
        float2 rayLength1D;
        int2 step;

        if (dir.x < 0)
        {
            step.x = -1;
            rayLength1D.x = (startPos.x - currentCell.x) * rayStepSize.x;
        }
        else
        {
            step.x = 1;
            rayLength1D.x = (currentCell.x + 1 - startPos.x) * rayStepSize.x;
        }
        
        if (dir.y < 0)
        {
            step.y = -1;
            rayLength1D.y = (startPos.y - currentCell.y) * rayStepSize.y;
        }
        else
        {
            step.y = 1;
            rayLength1D.y = (currentCell.y + 1 - startPos.y) * rayStepSize.y;
        }

        float distance = 0;
        float maxDistance = math.distance(start, end) / gridIntervall;
        while (maxDistance > distance && 
               currentCell.x >= -gridSize / gridIntervall && currentCell.x < gridSize / gridIntervall &&
               currentCell.y >= -gridSize / gridIntervall && currentCell.y < gridSize / gridIntervall)
        {
            int index = convertIndex(currentCell.x * gridIntervall, currentCell.y * gridIntervall);
            if (grid.value[index] == 0)
            {
                grid.value[index] = 1;
            }

            if (rayLength1D.x < rayLength1D.y)
            {
                currentCell.x += step.x;
                distance = rayLength1D.x;
                rayLength1D.x += rayStepSize.x;
            }
            else
            {
                currentCell.y += step.y;
                distance = rayLength1D.y;
                rayLength1D.y += rayStepSize.y;
            }
        }
        
    }
}
