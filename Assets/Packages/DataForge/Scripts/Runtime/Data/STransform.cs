using UnityEngine;

namespace DataForge.Data
{
    public struct STransform
    {
        public SVector3 position;
        public SVector3 rotation;
        public SVector3 scale;


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