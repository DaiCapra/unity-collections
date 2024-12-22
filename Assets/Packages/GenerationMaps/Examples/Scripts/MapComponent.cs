using GenerationMaps.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace GenerationMaps.Examples
{
    public class MapComponent : MonoBehaviour
    {
        public Image image;

        private void Start()
        {
            var width = 50;
            var height = 50;

            var generator = new TreeGenerator();
            var list = generator.Generate(width, height);

            ColorUtility.TryParseHtmlString("#166534", out var color);

            var painter = new TexturePainter(width, height);
            painter.Paint(list, color);
            painter.Apply(image);
        }
    }
}