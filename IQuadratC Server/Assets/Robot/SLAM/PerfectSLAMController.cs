using Unity.Mathematics;
using UnityEngine;
using Utility;

namespace Robot.SLAM
{
    public class PerfectSLAMController : MonoBehaviour
    {
        [SerializeField] private Transform robotTransform;
    
        [SerializeField] private PublicFloat2Array lidarDataPolar;
        [SerializeField] private PublicFloat2Array lidarData;
        [SerializeField] private PublicFloat3 pos;

        [SerializeField] private PublicInt minLidarDistance;
        [SerializeField] private PublicInt maxLidarDistance;
    
        [SerializeField] private PublicByteArray grid;
        [SerializeField] private PublicInt gridSize;
        [SerializeField] private PublicInt gridIntervall;

        private int convertIndex(float x, float y)
        {
            return (int) ((x + gridSize.value) / gridIntervall.value) * (gridSize.value / gridIntervall.value) * 2 + (int) (y + gridSize.value) / gridIntervall.value; 
        }

        void Start()
        {
            grid.value = new byte[gridSize.value * gridSize.value * 4 / (gridIntervall.value * gridIntervall.value)];
        }

        void Update()
        {
            pos.value = new float3(robotTransform.position.x, robotTransform.position.y, robotTransform.rotation.eulerAngles.z);
        
            int2 middleCell = (int2) (pos.value.xy / gridIntervall.value) * gridIntervall.value;
            for (int x = middleCell.x - maxLidarDistance.value; x < pos.value.x + maxLidarDistance.value; x += gridIntervall.value)
            {
                for (int y = middleCell.y - maxLidarDistance.value; y < pos.value.y + maxLidarDistance.value; y += gridIntervall.value)
                {
                    float2 polarPoint = mathAdditions.Cartisian2PolarDegree(new float2(x, y) - pos.value.xy);

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
                        int index = convertIndex(x, y);
                        if (grid.value[index] == 0)
                        {
                            grid.value[index] = 1;
                        }
                    }
                }
            }
        
            foreach (float2 point in lidarData.value)
            {
                if (math.distance(point, pos.value.xy) < minLidarDistance.value)
                {
                    continue;
                }
                int index = convertIndex(point.x, point.y);
                grid.value[index] = 2;
            }
        }
    }
}
