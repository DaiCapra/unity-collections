using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

public class ProjectToolsWindow : EditorWindow
{
    private static readonly Dictionary<string, Action> Buttons = new()
    {
        { "Cmd", ProjectTools.OpenCmd },
        { "Restart", ProjectTools.RestartEditor },
        { "Open Manifest", ProjectTools.OpenManifest },
        { "Open Folder", ProjectTools.OpenFolderAssets },

    };

    internal void OnGUI()
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

    [MenuItem("Tools/Project Tools")]
    internal static void Init()
    {
        var window = (ProjectToolsWindow)GetWindow(typeof(ProjectToolsWindow), false, "Project Tools");
        window.position = new(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 400f);
    }
}