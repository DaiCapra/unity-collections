using System;
using System.Linq;
using UnityEngine;

namespace SortComponents.SortComponents.Editor
{
    public class Sorter
    {
        private static string _namespaceEngine = "UnityEngine";

        public void Sort(GameObject g)
        {
            var components = GetComponents(g);

            if (components == null)
            {
                return;
            }

            var length = components.Length - 2;

            for (int j = 0; j <= length; j++)
            {
                for (int i = 0; i <= length; i++)
                {
                    var c = Compare(components[i], components[i + 1]);
                    if (c > 0)
                    {
                        var temp = components[i + 1];
                        components[i + 1] = components[i];
                        components[i] = temp;

                        UnityEditorInternal.ComponentUtility.MoveComponentUp(components[i]);
                    }
                }
            }
        }

        public Component[] GetComponents(GameObject g)
        {
            return g?.GetComponents<Component>()
                .Where(component => component.GetType() != typeof(Transform))
                .ToArray();
        }

        private int Compare(Component c1, Component c2)
        {
            var c = CompareEngine(c1, c2);
            if (c != 0)
            {
                return c;
            }

            var name1 = c1.GetType().Name;
            var name2 = c2.GetType().Name;

            return string.Compare(name1, name2, StringComparison.OrdinalIgnoreCase);
        }

        private int CompareEngine(Component c1, Component c2)
        {
            var e1 = IsEngineComponent(c1);
            var e2 = IsEngineComponent(c2);
            if (e1 && !e2)
            {
                return -1;
            }

            if (!e1 && e2)
            {
                return 1;
            }

            return 0;
        }

        public bool IsEngineComponent(Component c)
        {
            var type = c.GetType();
            return string.Equals(type.Namespace, _namespaceEngine, StringComparison.OrdinalIgnoreCase);
        }
    }
}