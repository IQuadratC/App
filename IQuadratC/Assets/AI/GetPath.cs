using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;
using Utility;

public class GetPath : MonoBehaviour
{
    [SerializeField]private bool active;
    [SerializeField]private Seeker seeker;
    [SerializeField]private PublicFloat2 start;
    [SerializeField]private PublicFloat2 goal;
    [SerializeField]private PublicFloat2Array path;
    [SerializeField]private PublicByteArray Map;
    [SerializeField]private PublicInt gridSize;
    [SerializeField]private PublicInt gridIntervall;
    
    
    private int convertIndex(float x, float y)
    {
        return (int) ((y + gridSize.value) / gridIntervall.value) * (gridSize.value / gridIntervall.value) * 2 + (int) (x + gridSize.value) / gridIntervall.value; 
    }
    
    void Start()
    {
        
    }


    void Update()
    {
        if (active)
        {
            explore();
            getPath();
            active = false;
        }
    }
    
    void explore()
    {
        for (int x = -gridSize.value; x < (gridSize.value - gridIntervall.value); x += gridIntervall.value)
        {
            for (int y = -gridSize.value; y < (gridSize.value - gridIntervall.value); y += gridIntervall.value)
            {
                if (Map.value[convertIndex(x, y)] == 0 && Map.value[convertIndex(x + gridIntervall.value, y)] == 1)
                {
                    updateGoal(x + gridIntervall.value, y);
                    continue;
                }

                if (Map.value[convertIndex(x, y)] == 0 && Map.value[convertIndex(x, y + gridIntervall.value)] == 1)
                {
                    updateGoal(x, y + gridIntervall.value);
                    continue;
                }

                if (Map.value[convertIndex(x, y)] == 1 && Map.value[convertIndex(x + gridIntervall.value, y)] == 0)
                {
                    updateGoal(x, y);
                    continue;
                }

                if (Map.value[convertIndex(x, y)] == 1 && Map.value[convertIndex(x, y + gridIntervall.value)] == 0)
                {
                    updateGoal(x, y);
                    continue;
                }
            }
        }
    }

    void updateGoal(int x, int y)
    {
        if (math.length(start.value - new int2(x, y)) < math.length(start.value - goal.value))
        {
            goal.value = new float2(x, y);
        }
    }
    void getPath()
    {
        seeker.StartPath(new Vector3(start.value.x, 0, start.value.y),
            new Vector3(goal.value.x, 0, goal.value.y), OnPathComplete);
        active = false;
    }

    void OnPathComplete(Path p)
    {
        path.value = new float2[p.vectorPath.Count];
        for (int i = 0; i < p.vectorPath.Count; i++)
        {
            path.value[i] = new float2(p.vectorPath[i].x,p.vectorPath[i].z);
        }
    } 
}
