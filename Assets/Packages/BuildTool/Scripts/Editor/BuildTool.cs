using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;
using Debug = UnityEngine.Debug;

namespace Repositories.BuildTool.Editor
{
    public class BuildTool : EditorWindow
    {
        private readonly JsonSerializerSettings _settings = new()
        {
            Formatting = Formatting.Indented
        };

        private readonly string _settingsPath = Path.Combine(Application.dataPath, "build-tool.json");

        private BuildSettings _buildSettings;

        public void OnGUI()
        {
            if (_buildSettings == null)
            {
                Load();
                _buildSettings ??= new BuildSettings();
            }

            GUILayout.Label("Build tools", EditorStyles.boldLabel);
            _buildSettings.productName = EditorGUILayout.TextField("Name: ", _buildSettings.productName);
            _buildSettings.buildFolder = EditorGUILayout.TextField("Build folder: ", _buildSettings.buildFolder);
            _buildSettings.buildName = EditorGUILayout.TextField("Exe name: ", _buildSettings.buildName);
            _buildSettings.version = EditorGUILayout.TextField("Version: ", _buildSettings.version);

            GUILayout.Space(10);
            GUILayout.Label("Scenes", EditorStyles.boldLabel);

            foreach (var scene in GetScenes())
            {
                GUILayout.Label(scene.path);
            }

            Save();

            GUILayout.Space(10);
            if (GUILayout.Button("Build"))
            {
                Build();
            }

            if (GUILayout.Button("Run"))
            {
                Run();
            }

            if (GUILayout.Button("Open"))
            {
                Open();
            }
        }

        private void Open()
        {
            if (string.IsNullOrEmpty(_buildSettings.buildFolder))
            {
                return;
            }

            Process.Start("explorer.exe", _buildSettings.buildFolder);
        }

        private void Run()
        {
            if (!File.Exists(GetExePath()))
            {
                return;
            }

            var proc = new Process();
            proc.StartInfo.FileName = GetExePath();
            proc.Start();
        }

        private static List<EditorBuildSettingsScene> GetScenes()
        {
            return EditorBuildSettings.scenes.Where(t => t.enabled).ToList();
        }

        private void Build()
        {
            BuildExe();
            Package();

            // Copy a file from the project folder to the build folder, alongside the built game.
            // FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");
        }

        private void Package()
        {
            ZipGameFiles();
            // WriteManifest();
        }

        private void ZipGameFiles()
        {
            var zipName = GetZipName();
            // var dir = GetBuildFolder();
            // if (!Directory.Exists(dir))
            // {
            //     Directory.CreateDirectory(dir);
            // }

            var zipPath = Path.Combine(_buildSettings.buildFolder, zipName);
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }


            ZipFile.CreateFromDirectory(
                GetReleaseFolder(),
                zipPath,
                CompressionLevel.Optimal,
                includeBaseDirectory: false);
        }

        private string GetZipName()
        {
            return $"build-{_buildSettings.version}.zip";
        }

        private void BuildExe()
        {
            var levels = GetScenes()
                .Select(t => t.path)
                .ToArray();

            var path = GetReleaseFolder();
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var exePath = GetExePath();
            BuildPipeline.BuildPlayer(levels, exePath, BuildTarget.StandaloneWindows, BuildOptions.None);
        }

        private string GetReleaseFolder()
        {
            return Path.Combine(_buildSettings.buildFolder, "release");
        }

        private string GetExePath()
        {
            var exePath = Path.Combine(GetReleaseFolder(), $"{_buildSettings.buildName}.exe");
            return exePath;
        }

        [MenuItem("Tools/Build Tool")]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(BuildTool)) as BuildTool;
            window?.Load();
        }

        private void Load()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var text = File.ReadAllText(_settingsPath);
                    _buildSettings = JsonConvert.DeserializeObject<BuildSettings>(text);

                    if (string.IsNullOrEmpty(_buildSettings.productName))
                    {
                        _buildSettings.productName = PlayerSettings.productName;
                    }

                    Repaint();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private void Save()
        {
            PlayerSettings.bundleVersion = _buildSettings.version;
            PlayerSettings.productName = _buildSettings.productName;
            var json = JsonConvert.SerializeObject(_buildSettings, _settings);
            File.WriteAllText(_settingsPath, json);
        }
    }
}