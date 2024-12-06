using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ProjectShortcuts.Editor
{
    public class ProjectShortcuts
    {
        public static void OpenCmd()
        {
            var dirRoot = GetDirectoryAssets();
            if (dirRoot is { Exists: true })
            {
                var processStartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = $"{dirRoot.FullName}",
                    FileName = "cmd.exe"
                };

                var proc = Process.Start(processStartInfo);
            }
        }

        public static void OpenFolderAssets()
        {
            var dirRoot = GetDirectoryAssets();
            if (dirRoot is { Exists: true })
            {
                OpenFolder(dirRoot.FullName);
            }
        }

        public static void OpenManifest()
        {
            var dirRoot = GetDirectoryAssets();
            if (dirRoot is { Exists: true })
            {
                var pathPackages = Path.Combine(dirRoot.FullName, "packages/manifest.json");
                if (File.Exists(pathPackages))
                {
                    var psi = new ProcessStartInfo(pathPackages)
                    {
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                else
                {
                    Debug.LogError($"File doesn't exist: {pathPackages}");
                }
            }
        }

        public static void RestartEditor()
        {
            EditorApplication.OpenProject(Directory.GetCurrentDirectory());
        }

        private static DirectoryInfo GetDirectoryAssets()
        {
            var pathAssets = Application.dataPath;
            var dirRoot = Directory.GetParent(pathAssets);
            return dirRoot;
        }

        private static void OpenFolder(string path)
        {
            Process.Start("explorer.exe", path);
        }
    }
}