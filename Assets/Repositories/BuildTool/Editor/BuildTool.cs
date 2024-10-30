using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace Repositories.BuildTool.Editor
{
    public class BuildSettings
    {
        public string buildFolder = "Build";
        public string buildName = "Game";
        public string version = "1.0.0";
    }

    public class BuildTool : EditorWindow
    {
        private readonly JsonSerializerSettings _settings = new()
        {
            Formatting = Formatting.Indented
        };

        private BuildSettings _buildSettings = new();

        private string _settingsPath = Path.Combine(Application.dataPath, "build-settings.json");

        public void OnGUI()
        {
            GUILayout.Label("Build tools", EditorStyles.boldLabel);
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
            WriteManifest();
        }

        private void WriteManifest()
        {
            var manifest = new Manifest() { name = GetZipName(), version = _buildSettings.version };

            var json = JsonConvert.SerializeObject(manifest, _settings);

            var path = Path.Combine(GetBuildFolder(), "manifest.json");
            File.WriteAllText(path, json);
        }

        private void ZipGameFiles()
        {
            var zipName = GetZipName();
            var dir = GetBuildFolder();
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var zipPath = Path.Combine(dir, zipName);
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

        private string GetBuildFolder()
        {
            return Path.Combine(_buildSettings.buildFolder, $"build-{_buildSettings.version}");
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
                    Repaint();
                }
            }
            catch
            {
            }
        }

        private void Save()
        {
            PlayerSettings.bundleVersion = _buildSettings.version;

            var json = JsonConvert.SerializeObject(_buildSettings, _settings);
            File.WriteAllText(_settingsPath, json);
        }
    }
}