using UnityEditor;
using UnityEngine;

namespace SortComponents.SortComponents.Editor
{
    public static class SortComponents
    {
        [MenuItem("CONTEXT/Component/Sort Components")]
        public static void Sort(MenuCommand command)
        {
            var g = (command.context as Component)?.gameObject;
            var sorter = new Sorter();
            sorter.Sort(g);
        }
    }
}