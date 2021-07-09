using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;

public class GetPath : MonoBehaviour
{
    [SerializeField]
    private bool active;
    [SerializeField]
    private Seeker seeker;
    [SerializeField]
    private float3 start;
    [SerializeField]
    private float3 goal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            seeker.StartPath(start, goal, OnPathComplete);
            active = false;
        }
    }

    void OnPathComplete(Path p)
    {
        foreach (var vector3 in p.vectorPath)
        {
            Debug.Log(vector3);
        }
    } 
}
