using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;
using Utility;

public class AISetup : MonoBehaviour
{
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;
    [SerializeField]
    private bool active;
    [SerializeField]
    private PublicInt2Array obsticals;
    
    // Start is called before the first frame update
    void Start()
    {
        GridGraph gg = AstarPath.active.data.AddGraph(typeof(GridGraph)) as GridGraph;
        gg.center = new Vector3(0, 0, 0);
        gg.neighbours = NumNeighbours.Eight;
    }

    // Update is called once per frame
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
        gg.SetDimensions(width, height,1); // nodeSize is 1 because we dont use it
        AstarPath.active.Scan();
        AstarPath.active.AddWorkItem(new AstarWorkItem(ctx => {
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    var node = gg.GetNode(x,y);
                    node.Walkable = !obsticals.value.Contains(new int2(x,y));
                }
            }
            gg.GetNodes(node => gg.CalculateConnections((GridNodeBase)node));
        }));
    }
}
