using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace DataForge.ResourcesManagement
{
    public class ResourceManager : IResourceProvider
    {
        public ResourceManager()
        {
        }

        public Dictionary<string, Object> Resources { get; } = new(StringComparer.OrdinalIgnoreCase);

        public void Clear()
        {
            Resources.Clear();
        }

        public async Task Load(string label)
        {
            var addressables = await LoadAddressables(label);
            foreach (var kv in addressables)
            {
                Resources[kv.Key] = kv.Value;
            }
        }

        private async Task<Dictionary<string, Object>> LoadAddressables(string label)
        {
            var map = new Dictionary<string, Object>(StringComparer.OrdinalIgnoreCase);

            var locations = await Addressables.LoadResourceLocationsAsync(label).Task;
            if (!locations.Any())
            {
                return map;
            }

            var tasks = locations.Select(async location =>
            {
                var list = new List<IResourceLocation>() { location };
                var handle = Addressables.LoadAssetsAsync<Object>(locations: list, callback: null);
                await handle.Task;

                return (location, handle);
            }).ToList();

            var result = await Task.WhenAll(tasks);
            foreach (var kv in result)
            {
                var location = kv.location;
                var handle = kv.handle;
                if (handle.Status is not AsyncOperationStatus.Succeeded)
                {
                    continue;
                }

                var key = location.PrimaryKey;
                var obj = handle.Result.FirstOrDefault();
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                map[key] = obj;
            }

            return map;
        }
    }
}