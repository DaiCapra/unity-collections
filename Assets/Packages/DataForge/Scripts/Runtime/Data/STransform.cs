using UnityEngine;

namespace DataForge.Data
{
    public struct STransform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public static implicit operator STransform(Transform t)
        {
            return new()
            {
                position = t.position,
                rotation = t.rotation,
                scale = t.localScale
            };
        }

        public void Apply(Transform transform)
        {
            transform.position = position;
            transform.rotation = rotation;
            transform.localScale = scale;
        }
    }
}