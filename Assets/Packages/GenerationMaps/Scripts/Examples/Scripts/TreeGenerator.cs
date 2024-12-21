using System.Collections.Generic;
using UnityEngine;

namespace GenerationMaps.Examples
{
    public class TreeGenerator
    {
        public List<Vector2Int> Generate(int width, int height)
        {
            var list = new List<Vector2Int>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var place = Random.Range(0, 10) > 5;
                    if (place)
                    {
                        list.Add(new(x, y));
                    }
                }
            }

            return list;
        }
    }
}