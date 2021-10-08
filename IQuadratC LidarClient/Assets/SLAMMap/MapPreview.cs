using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class MapPreview : MonoBehaviour
{
    [SerializeField] private PublicByteArray grid;
    [SerializeField] private PublicInt gridSize;
    [SerializeField] private PublicInt gridIntervall;
    
    private int convertIndex(float x, float y)
    {
        return (int) (y / gridIntervall.value) * (gridSize.value / gridIntervall.value) + (int) (x / gridIntervall.value); 
    }
    
    private void OnDrawGizmos()
    {
        if (grid != null && grid.value.Length > 0)
        {
            for (int x = 0; x < gridSize.value; x += gridIntervall.value)
            {
                for (int y = 0; y < gridSize.value; y += gridIntervall.value)
                {
                    byte gridCell = 0;
                    try
                    {
                        gridCell = grid.value[convertIndex(x, y)];
                    }
                    catch (Exception e)
                    {
                        Debug.LogFormat("{0}, {0}", x, y);
                    }
                    
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
                    Gizmos.DrawCube(new Vector3(x + gridIntervall.value,y + gridIntervall.value, 20), Vector3.one * 2);
                }
            }
        }
    }
}
