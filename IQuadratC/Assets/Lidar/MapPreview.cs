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
        return (int) ((x + gridSize.value) / gridIntervall.value) * (gridSize.value / gridIntervall.value) * 2 + (int) (y + gridSize.value) / gridIntervall.value; 
    }
    
    private void OnDrawGizmos()
    {
        if (grid != null && grid.value.Length > 0)
        {
            for (int x = -gridSize.value; x < gridSize.value; x += gridIntervall.value)
            {
                for (int y = -gridSize.value; y < gridSize.value; y += gridIntervall.value)
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
                    Gizmos.DrawCube(new Vector3(x + gridIntervall.value / 2, 20, y + gridIntervall.value / 2), Vector3.one * 2);
                }
            }
        }
    }
}
