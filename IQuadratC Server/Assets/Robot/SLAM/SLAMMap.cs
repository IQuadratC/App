using Unity.Mathematics;
using Utility;

namespace Robot.SLAM
{
    public class SLAMMap
    {
        public byte[][] maps;
        public int intervall;
        public int dimentions;
        
        public SLAMMap(int dimentions, int intervall, int levels)
        {
            this.intervall = intervall;
            this.dimentions = dimentions;
            
            maps = new byte[levels][];

            for (int i = 0; i < levels; i++)
            {
                int size = dimentions * 2 / getIntervall(i);
                maps[i] = new byte[size * size];
            }
        }

        private bool boundsCheck(float2 pos)
        {
            return pos.x < -dimentions || pos.x >= dimentions || pos.y < -dimentions || pos.y >= dimentions;
        }

        private int getIndex(float2 cell, int inter)
        {
            return (int) ((cell.x + dimentions) / inter) * (dimentions / inter) * 2  + (int) ((cell.y + dimentions) / inter);
        }

        private int getIntervall(int level)
        {
            int inter = intervall;
            for (int i = 0; i < level; i++)
            {
                inter *= 2;
            }
            return inter;
        }
        
        public byte GetByte(float2 pos, int level)
        {
            if (boundsCheck(pos)) { return 0; }
            
            return maps[level][getIndex(pos, getIntervall(level))];
        }
        
        public void SetByte(float2 pos, byte value)
        {
            if (boundsCheck(pos)) { return; }
            
            for (int i = 0; i < maps.Length; i++)
            {
                if (maps[i].Length == 0) {continue;}
                maps[i][getIndex(pos, getIntervall(i))] = value;
            }
        }
    }
}