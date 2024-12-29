using Unity.Properties;
using UnityEngine.UIElements;

namespace DataForge.Editor
{
    public static class VisualElementExtensions
    {
        public static void Bind(
            this VisualElement visualElement,
            object source,
            string propertyName,
            string path
        )
        {
            visualElement.dataSource = source;
            visualElement.dataSourcePath = new PropertyPath(path);
            visualElement.SetBinding(propertyName, new DataBinding());
        }
    }
}