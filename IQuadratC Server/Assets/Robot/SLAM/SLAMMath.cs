using Unity.Mathematics;

namespace Robot.SLAM
{
    public class SLAMMath
    {
        public static float MapAcsess(float2 p, SLAMMap map, int level)
        {
            int2 p0 = (int2) (p / map.intervall);
            int2 p1 = p0 + new int2(1,1);
            p = p / map.intervall;
            float a = p.y - p0.y;
            float b = p1.y - p.y;
            float c = p.x - p0.x;
            float d = p1.x - p.x;
            
            float m00 = map.GetByte(p0 *  map.intervall, level) - 1;
            float m01 = map.GetByte(new int2(p0.x, p1.y) *  map.intervall, level) - 1;
            float m10 = map.GetByte(new int2(p1.x, p0.y) *  map.intervall, level) - 1;
            float m11 = map.GetByte(p1 *  map.intervall, level) - 1;

            float x = a * (c * m11 + d * m01)
                      + b * (c * m10 + d * m00);
            return x;
        }
        
        public static float2 MapAcsessDirtative(float2 p, SLAMMap map, int level)
        {
            int2 p0 = (int2) p / map.intervall;
            int2 p1 = p0 + new int2(1, 1);
            p = p / map.intervall;
            float a = p.y - p0.y;
            float b = p1.y - p.y;
            float c = p.x - p0.x;
            float d = p1.x - p.x;
            
            float m00 = map.GetByte(p0 * map.intervall, level);
            float m01 = map.GetByte(new int2(p0.x, p1.y) * map.intervall, level);
            float m10 = map.GetByte(new int2(p1.x, p0.y) * map.intervall, level);
            float m11 = map.GetByte(p1 * map.intervall, level);

            float2 x = new float2(
                a * (m11 - m01) + b * (m10 - m00),
                c * (m11 - m01) + d * (m10 - m00));
            return x;
        }
    }
}