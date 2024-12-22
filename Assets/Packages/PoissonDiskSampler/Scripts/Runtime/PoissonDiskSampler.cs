using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PoissonDiskSampler.Runtime
{
    public static class PoissonDiskSampler
    {
        /// <summary>
        /// Generates a list of points using Poisson Disk Sampling.
        /// </summary>
        /// <param name="width">Width of the sampling area.</param>
        /// <param name="height">Height of the sampling area.</param>
        /// <param name="radius">Minimum distance between points.</param>
        /// <param name="k">Number of attempts to generate a valid point near an existing point.</param>
        public static List<Vector2> Sample(float width, float height, float radius, int k = 30)
        {
            if (radius <= 0)
            {
                throw new Exception("Radius must be greater than 0");
            }
            
            // Size of each grid cell
            var cellSize = radius / Mathf.Sqrt(2);
            var gridWidth = Mathf.CeilToInt(width / cellSize);
            var gridHeight = Mathf.CeilToInt(height / cellSize);

            var grid = new Vector2[gridWidth, gridHeight];
            var points = new List<Vector2>();
            var spawnPoints = new List<Vector2>();

            // Add the initial point
            spawnPoints.Add(new Vector2(Random.Range(0, width), Random.Range(0, height)));

            while (spawnPoints.Count > 0)
            {
                // Pick a random spawn point
                var spawnIndex = Random.Range(0, spawnPoints.Count);
                var spawnCenter = spawnPoints[spawnIndex];
                var pointFound = false;

                // Try to generate a valid point near the spawn point
                for (var i = 0; i < k; i++)
                {
                    var angle = Random.Range(0, Mathf.PI * 2);
                    var distance = Random.Range(radius, 2 * radius);
                    var candidate = spawnCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

                    if (IsValid(candidate, width, height, radius, cellSize, grid, gridWidth, gridHeight))
                    {
                        points.Add(candidate);
                        spawnPoints.Add(candidate);
                        grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = candidate;
                        pointFound = true;
                        break;
                    }
                }

                // Remove the spawn point if no valid points were found
                if (!pointFound)
                {
                    spawnPoints.RemoveAt(spawnIndex);
                }
            }

            return points;
        }

        private static bool IsValid(
            Vector2 candidate,
            float width,
            float height,
            float radius,
            float cellSize,
            Vector2[,] grid,
            int gridWidth,
            int gridHeight
        )
        {
            // Check if the candidate is within bounds
            if (candidate.x < 0 || candidate.x >= width || candidate.y < 0 || candidate.y >= height)
            {
                return false;
            }

            // Get the grid cell of the candidate
            var cellX = (int)(candidate.x / cellSize);
            var cellY = (int)(candidate.y / cellSize);

            // Check neighboring cells
            var minX = Mathf.Max(0, cellX - 2);
            var maxX = Mathf.Min(gridWidth - 1, cellX + 2);
            var minY = Mathf.Max(0, cellY - 2);
            var maxY = Mathf.Min(gridHeight - 1, cellY + 2);

            for (var x = minX; x <= maxX; x++)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    var neighbor = grid[x, y];
                    var distance = Vector2.Distance(candidate, neighbor);
                    if (neighbor != Vector2.zero && distance < radius)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}