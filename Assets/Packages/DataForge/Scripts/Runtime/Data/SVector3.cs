using UnityEngine;

namespace DataForge.Data
{
    public struct SVector3
    {
        public float x;
        public float y;
        public float z;

        public static implicit operator SVector3(Vector3 v)
        {
            return new SVector3()
            {
                x = v.x,
                y = v.y,
                z = v.z
            };
        }

        public static implicit operator Vector3(SVector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static SVector3 One = new() { x = 1, y = 1, z = 1 };

        public override string ToString()
        {
            return $"[{x}, {y}, {z}]";
        }
    }
}