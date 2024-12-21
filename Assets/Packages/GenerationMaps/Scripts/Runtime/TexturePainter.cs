using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GenerationMaps.Runtime
{
    public class TexturePainter
    {
        public Color defaultColor = Color.black;
        public int height;
        public Texture2D texture;
        public int width;

        public TexturePainter(int width, int height)
        {
            this.width = width;
            this.height = height;

            Create();
        }

        public void Apply(Image image)
        {
            var sprite = ToSprite(texture);

            image.enabled = true;
            image.sprite = sprite;
            image.preserveAspect = true;
        }

        public void Paint(List<Vector2Int> list, Color color)
        {
            foreach (var pos in list)
            {
                texture.SetPixel(pos.x, pos.y, color);
            }

            texture.Apply();
        }

        private void Create()
        {
            texture = new Texture2D(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, defaultColor);
                }
            }

            texture.Apply();
        }

        private Sprite ToSprite(Texture2D texture)
        {
            var sprite = Sprite.Create(
                texture,
                rect: new Rect(0, 0, texture.width, texture.height),
                pivot: new Vector2(0.5f, 0.5f),
                pixelsPerUnit: 100f
            );

            return sprite;
        }
    }
}