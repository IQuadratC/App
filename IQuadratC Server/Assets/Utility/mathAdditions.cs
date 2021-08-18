using System;
using Unity.Mathematics;
using UnityEngine;

namespace Utility
{
    public static class mathAdditions
    {
        public static float2 Cartisian2PolarDegree(float2 point)
        {
            return new float2(math.degrees(math.atan2(point.y, point.x)),math.sqrt(point.x * point.x + point.y * point.y));
        }
        
        public static float2 Polar2CartisianDegree(float2 point)
        {
            float angleRad = math.radians(point.x);
            return new float2(point.y * math.cos(angleRad), point.y * math.sin(angleRad));
        }
    }
}