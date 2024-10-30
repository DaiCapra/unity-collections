using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ProjectTools
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

    public static void RestartEditor()
    {
        EditorApplication.OpenProject(Directory.GetCurrentDirectory());
    }

    public static void OpenFolderAssets()
    {
        var dirRoot = GetDirectoryAssets();
        if (dirRoot is { Exists: true })
        {
            OpenFolder(dirRoot.FullName);
        }
    }

    public static void OpenFolder(string path)
    {
        Process.Start("explorer.exe", path);
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

    private static DirectoryInfo GetDirectoryAssets()
    {
        var pathAssets = Application.dataPath;
        var dirRoot = Directory.GetParent(pathAssets);
        return dirRoot;
    }
}