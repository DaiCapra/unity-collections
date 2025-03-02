using Arch.Core;
using DataForge.Data;
using UnityEngine;

namespace DataForge.Objects
{
    public interface IObjectManager
    {
        bool LinkToPrefab { get; set; }

        GameObject Make(
            GameObject prefab,
            OptionalValue<Vector3> position = default,
            OptionalValue<Vector3> rotation = default,
            OptionalValue<Entity> entity = default,
            Transform parent = default,
            bool isActive = true);

        void Unmake(GameObject gameObject);
        GameObject InstantiateGameObject(GameObject prefab);
    }
}