using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class SLAMController : MonoBehaviour
{
    [SerializeField] private PublicFloat2Array liveData;

    private byte[] grid;

    private int convertIndex(float x, float y)
    {
        return (int) ((x + gridSize) * gridSize * 2 / gridIntervall)  + (int) ((y + gridSize) / gridIntervall); 
    }

    [SerializeField] private int gridSize = 1000;
    [SerializeField] private int gridIntervall = 10; 
    
    void Start()
    {
        grid = new byte[gridSize * gridSize * 4 / gridIntervall];
    }

    private void OnDrawGizmos()
    {
        if (grid != null && grid.Length > 0)
        {
            for (int x = -gridSize; x < gridSize; x += gridIntervall)
            {
                for (int y = -gridSize; y < gridSize; y += gridIntervall)
                {
                    byte gridCell = grid[convertIndex(x, y)];
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
                    Gizmos.DrawCube(new Vector3(x, 0, y), Vector3.one * 3);
                }
            }
        }
    }

    void Update()
    {
        
    }
}
