using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;
using Utility;

public class GetPath : MonoBehaviour
{
    [SerializeField]
    private bool active;
    [SerializeField]
    private Seeker seeker;
    [SerializeField]
    private PublicFloat2 start;
    [SerializeField]
    private PublicFloat2 goal;
    [SerializeField]
    private PublicFloat2Array path;

    void Start()
    {
        
    }


    void Update()
    {
        if (active)
        {
            seeker.StartPath(new Vector3(start.value.x, 0, start.value.y),
                new Vector3(goal.value.x, 0, goal.value.y), OnPathComplete);
            active = false;
        }
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
