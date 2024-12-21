using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProjectShortcuts.Scripts.Editor
{
    public class ProjectShortcutsWindow : EditorWindow
    {
        private static readonly Dictionary<string, Action> Buttons = new()
        {
            { "Cmd", ProjectShortcuts.OpenCmd },
            { "Restart", ProjectShortcuts.RestartEditor },
            { "Open Manifest", ProjectShortcuts.OpenManifest },
            { "Open Folder", ProjectShortcuts.OpenFolderAssets },
        };

        private void OnGUI()
        {
            if (Buttons == null)
            {
                return;
            }

            EditorGUILayout.BeginVertical();
            foreach (var kv in Buttons)
            {
                var name = kv.Key;
                var action = kv.Value;

                if (GUILayout.Button(name))
                {
                    action.Invoke();
                }
            }

            EditorGUILayout.EndVertical();
        }

        [MenuItem("Tools/Project Shortcuts")]
        internal static void Init()
        {
            var window = (ProjectShortcutsWindow)GetWindow(typeof(ProjectShortcutsWindow), false, "Project Shortcuts");
        }
    }
}