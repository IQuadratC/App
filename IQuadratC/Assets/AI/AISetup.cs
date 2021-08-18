using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

public class AISetup : MonoBehaviour
{
    [SerializeField]private bool active;
    [SerializeField]private PublicByteArray Map;
    [SerializeField]private PublicInt gridSize;
    [SerializeField]private PublicInt gridIntervall;
    private int size;

    void Start()
    {
        GridGraph gg = AstarPath.active.data.AddGraph(typeof(GridGraph)) as GridGraph;
        gg.center = new Vector3(0.5f, 0, 0.5f); // 0,5 so cell coordinates are integers. 
        gg.neighbours = NumNeighbours.Eight;
    }


    void Update()
    {
        if (active)
        {
            UpdateGraph();
            active = false;
        }
    }

    void UpdateGraph()
    {
        GridGraph gg = AstarPath.active.data.gridGraph;
        gg.SetDimensions(gridSize.value/gridIntervall.value * 2, gridSize.value/gridIntervall.value * 2, gridIntervall.value);
        AstarPath.active.Scan();
        AstarPath.active.AddWorkItem(new AstarWorkItem(ctx => {
            for (int i = 0; i < Map.value.Length; i++)
            {
                gg.nodes[i].Walkable = Map.value[i] == 1;
            }
            gg.GetNodes(node => gg.CalculateConnections((GridNodeBase)node));
        }));
    }
}
