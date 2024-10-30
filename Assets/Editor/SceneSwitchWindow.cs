using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    // --------------------------------
// <copyright file="SceneSwitchWindow.cs" company="Rumor Games">
//     Copyright (C) Rumor Games, LLC.  All rights reserved.
// </copyright>
// --------------------------------
/// <summary>
    /// SceneSwitchWindow class.
    /// </summary>
    public class SceneSwitchWindow : EditorWindow
    {
        /// <summary>
        /// Tracks scroll position.
        /// </summary>
        private Vector2 scrollPos;

        /// <summary>
        /// Initialize window state.
        /// </summary>
        [MenuItem("Tools/Scene Switch Window")]
        internal static void Init()
        {
            // EditorWindow.GetWindow() will return the open instance of the specified window or create a new
            // instance if it can't find one. The second parameter is a flag for creating the window as a
            // Utility window; Utility windows cannot be docked like the Scene and Game view windows.
            var window = (SceneSwitchWindow) GetWindow(typeof(SceneSwitchWindow), false, "Scene Switch");
            window.position = new(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 400f);
        }

        /// <summary>
        /// Called on GUI events.
        /// </summary>
        internal void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

            GUILayout.Label("Scenes In Build", EditorStyles.boldLabel);
            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var scene = EditorBuildSettings.scenes[i];
                if (scene.enabled)
                {
                    var sceneName = Path.GetFileNameWithoutExtension(scene.path);
                    var guiStyle = new GUIStyle(GUI.skin.GetStyle("Button")) {alignment = TextAnchor.MiddleLeft};

                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button(i + ": " + sceneName, guiStyle))
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            EditorSceneManager.OpenScene(scene.path);
                        }
                    }

                    if (GUILayout.Button("→", GUILayout.MaxWidth(20)))
                    {
                        var p = scene.path;
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath(p, typeof(Object));
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}