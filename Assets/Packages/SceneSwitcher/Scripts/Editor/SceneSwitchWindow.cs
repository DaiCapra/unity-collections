using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SceneSwitcher.Scripts.Editor
{
    public class SceneSwitchWindow : EditorWindow
    {
        private Vector2 _scrollPos;

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);

            GUILayout.Label("Scenes", EditorStyles.boldLabel);
            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var scene = EditorBuildSettings.scenes[i];
                if (scene.enabled)
                {
                    var sceneName = Path.GetFileNameWithoutExtension(scene.path);
                    var guiStyle = new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft };

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

        [MenuItem("Tools/Scene Switch Window")]
        internal static void Init()
        {
            var window = (SceneSwitchWindow)GetWindow(typeof(SceneSwitchWindow), false, "Scene Switch");
        }
    }
}