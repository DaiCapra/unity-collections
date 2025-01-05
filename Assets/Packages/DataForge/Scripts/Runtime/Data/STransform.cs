using UnityEngine;

namespace DataForge.Data
{
    public struct STransform
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public static implicit operator STransform(Transform t)
        {
            return new()
            {
                position = t.position,
                rotation = t.rotation.eulerAngles,
                scale = t.localScale
            };
        }

        public void Apply(Transform transform)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(rotation);
            transform.localScale = scale;
        }
    }
}