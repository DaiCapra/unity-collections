using Arch.Core;
using DataForge.Data;
using UnityEditor;
using UnityEngine;

namespace DataForge.Objects
{
    public class ObjectManager: IObjectManager
    {
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
            gameObject.name = prefab.name;

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
            OnMake(gameObject);
            
            return gameObject;
        }

        public void Unmake(GameObject gameObject)
        {
            if (gameObject != null)
            {
                Object.Destroy(gameObject);
            }
        }

        protected virtual void OnMake(GameObject gameObject)
        {
        }

        public GameObject InstantiateGameObject(GameObject prefab)
        {
#if UNITY_EDITOR
            var gameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
#else
            var gameObject = Object.Instantiate(prefab);
#endif
            return gameObject;
        }
    }
}