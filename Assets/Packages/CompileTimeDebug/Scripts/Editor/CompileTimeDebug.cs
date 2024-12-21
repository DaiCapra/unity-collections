using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace CompileTimeDebug.Scripts.Editor
{
    [InitializeOnLoad]
    public class CompileTimeDebug
    {
        private const bool Log = true;
        private const string AssemblyReloadEventsEditorPref = "AssemblyReloadEventsTime";
        private const string AssemblyCompilationEventsEditorPref = "AssemblyCompilationEvents";
        private static readonly int ScriptAssembliesPathLen = "Library/ScriptAssemblies/".Length;

        private static readonly Dictionary<string, DateTime> StartTimes = new();

        private static readonly StringBuilder BuildEvents = new();
        private static double _compilationTotalTime;

        static CompileTimeDebug()
        {
#pragma warning disable CS0618
            CompilationPipeline.assemblyCompilationStarted += CompilationPipelineOnAssemblyCompilationStarted;
#pragma warning restore CS0618
            CompilationPipeline.assemblyCompilationFinished += CompilationPipelineOnAssemblyCompilationFinished;
            AssemblyReloadEvents.beforeAssemblyReload += AssemblyReloadEventsOnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += AssemblyReloadEventsOnAfterAssemblyReload;
        }

        static void AssemblyReloadEventsOnAfterAssemblyReload()
        {
            var binString = EditorPrefs.GetString(AssemblyReloadEventsEditorPref);

            if (long.TryParse(binString, out var bin))
            {
                var date = DateTime.FromBinary(bin);
                var time = DateTime.UtcNow - date;
                var compilationTimes = EditorPrefs.GetString(AssemblyCompilationEventsEditorPref);

                if (!string.IsNullOrEmpty(compilationTimes))
                {
                    if (Log)
                    {
                        Debug.Log("Assembly Reload Time: " + time.TotalSeconds + "s\n");
                    }
                }
            }
        }

        private static void AssemblyReloadEventsOnBeforeAssemblyReload()
        {
            BuildEvents.AppendFormat("Compilation total: {0:0.00}s\n", _compilationTotalTime / 1000f);
            EditorPrefs.SetString(AssemblyReloadEventsEditorPref, DateTime.UtcNow.ToBinary().ToString());
            EditorPrefs.SetString(AssemblyCompilationEventsEditorPref, BuildEvents.ToString());
        }

        private static void CompilationPipelineOnAssemblyCompilationFinished(string assembly, CompilerMessage[] arg2)
        {
            var timeSpan = DateTime.UtcNow - StartTimes[assembly];
            _compilationTotalTime += timeSpan.TotalMilliseconds;

            BuildEvents.AppendFormat("{0:0.00}s {1}\n", timeSpan.TotalMilliseconds / 1000f,
                assembly.Substring(ScriptAssembliesPathLen, assembly.Length - ScriptAssembliesPathLen));
        }

        private static void CompilationPipelineOnAssemblyCompilationStarted(string assembly)
        {
            StartTimes[assembly] = DateTime.UtcNow;
        }
    }
}