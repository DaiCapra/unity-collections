using Arch.Core;
using DataForge.Data;
using UnityEditor;
using UnityEngine;

namespace DataForge.Objects
{
    public class ObjectManager : IObjectManager
    {
        public bool LinkToPrefab { get; set; } = true;

        public GameObject Make(
            GameObject prefab,
            OptionalValue<Vector3> position = default,
            OptionalValue<Vector3> rotation = default,
            OptionalValue<Entity> entity = default,
            Transform parent = default,
            bool isActive = true)
        {
            if (prefab == null)
            {
                Debug.LogError("Prefab is null");
                return null;
            }

            var gameObject = InstantiateGameObject(prefab);
            // gameObject.name = prefab.name;

            if (position)
            {
                gameObject.transform.position = position;
            }

            if (rotation)
            {
                gameObject.transform.rotation = Quaternion.Euler(rotation);
            }

            if (parent)
            {
                gameObject.transform.parent = parent;
            }

            if (entity)
            {
                var actor = gameObject.GetComponent<Actor>() != null
                    ? gameObject.GetComponent<Actor>()
                    : gameObject.AddComponent<Actor>();

                actor.entity = entity;
            }

            gameObject.SetActive(isActive);
            OnMake(gameObject, entity);

            return gameObject;
        }

        public void Unmake(GameObject gameObject)
        {
            if (gameObject != null)
            {
                Object.Destroy(gameObject);
            }
        }

        public GameObject InstantiateGameObject(GameObject prefab)
        {
            GameObject gameObject = null;
            if (LinkToPrefab)
            {
#if UNITY_EDITOR
                gameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
#else
                gameObject = Object.Instantiate(prefab);
#endif
            }
            else
            {
                gameObject = Object.Instantiate(prefab);
            }

            return gameObject;
        }

        protected virtual void OnMake(GameObject gameObject, OptionalValue<Entity> entity = default)
        {
            
        }
    }
}