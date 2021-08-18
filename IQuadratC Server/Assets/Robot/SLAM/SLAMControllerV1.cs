using System;
using Unity.Mathematics;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Robot.SLAM
{
    public class SLAMControllerV1 : MonoBehaviour
    {
        [SerializeField] private PublicFloat2Array lidarDataPolar;
        [SerializeField] private PublicFloat2Array lidarData;
        [SerializeField] private PublicFloat3 pos;

        [SerializeField] private PublicInt minLidarDistance;
        [SerializeField] private PublicInt maxLidarDistance;
    
        [SerializeField] private PublicByteArray grid;
        [SerializeField] private PublicInt gridSize;
        [SerializeField] private PublicInt gridIntervall;

        [SerializeField] private int levels = 3;

        private SLAMMap map;
        private void Awake()
        {
            map = new SLAMMap(gridSize.value, gridIntervall.value, levels);
        }

        private void Update()
        {
            if (lidarDataPolar.value.Length == 0)
            {
                return;
            }
            
            int2 middleCell = (int2) (pos.value.xy / gridIntervall.value) * gridIntervall.value;
            for (int x = middleCell.x - maxLidarDistance.value; x < pos.value.x + maxLidarDistance.value; x += gridIntervall.value)
            {
                for (int y = middleCell.y - maxLidarDistance.value; y < pos.value.y + maxLidarDistance.value; y += gridIntervall.value)
                {
                    int2 point = new int2(x, y);
                    if (map.GetByte(point, 0) != 0) { continue; }
                    
                    float2 polarPoint = mathAdditions.Cartisian2PolarDegree(point - pos.value.xy);

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
                        map.SetByte(new float2(x,y), 1);
                    }
                }
            }
            
        
            
            foreach (float2 point in lidarData.value)
            {
                if (math.distance(point, pos.value.xy) < minLidarDistance.value)
                {
                    continue;
                }
                map.SetByte(point, 2);
            }
            grid.value = map.maps[0];
        }

        private void OnDrawGizmos()
        {
            if (map == null) {return;}
            
            Gizmos.color = new Color(1, 0, 0, 0.3f);

            for (int i = 0; i < 100000; i++)
            {
                float2 pos = new float2(Random.Range(-gridSize.value, gridSize.value),
                    Random.Range(-gridSize.value, gridSize.value));
                
                float value1 = SLAMMath.MapAcsess(pos, map, 0);
                Gizmos.DrawCube(new float3(pos, -30), Vector3.one * value1 * 2);

                float2 value2 = SLAMMath.MapAcsessDirtative(pos, map, 0);
                //Gizmos.DrawLine(new float3(x, y, -30), new float3(x, y, -30) + new float3(value2, 0) * 10);
            }
        }
    }
}