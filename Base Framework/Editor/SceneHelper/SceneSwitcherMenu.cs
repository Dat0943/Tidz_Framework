#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tidz.Editor
{
    [InitializeOnLoad]
    public static class SceneSwitcherMenu
    {
        private const string MenuRoot = "Tidz/Switch Scene/";
        private const int MenuPriority = 200;

        private const bool IncludeAllProjectScenes = true;

        private static readonly List<string> RegisteredPaths = new();

        private static readonly MethodInfo AddMenuItemMethod;
        private static readonly MethodInfo RemoveMenuItemMethod;

        static SceneSwitcherMenu()
        {
            var menuType = typeof(Menu);

            AddMenuItemMethod = menuType.GetMethod(
                "AddMenuItem",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[] { typeof(string), typeof(string), typeof(bool), typeof(int), typeof(Action), typeof(Func<bool>) },
                null);

            RemoveMenuItemMethod = menuType.GetMethod(
                "RemoveMenuItem",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[] { typeof(string) },
                null);

            RequestRebuild();
            EditorBuildSettings.sceneListChanged += RequestRebuild;
        }

        internal static void RequestRebuild()
        {
            EditorApplication.delayCall -= Rebuild;
            EditorApplication.delayCall += Rebuild;
        }

        private static void Rebuild()
        {
            if (AddMenuItemMethod == null)
                return;

            ClearRegistered();

            var usedNames = new HashSet<string>();
            var scenePaths = CollectScenePaths();
            int index = 0;

            foreach (var scenePath in scenePaths)
            {
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                string uniqueName = sceneName;
                int dup = 1;

                while (!usedNames.Add(uniqueName))
                {
                    dup++;
                    uniqueName = $"{sceneName} ({dup})";
                }

                string menuPath = MenuRoot + uniqueName;
                string capturedPath = scenePath;

                Action execute = () => OpenScene(capturedPath);

                // KHÔNG gọi Menu.RebuildAllMenus() ở đây:
                // nó dựng lại menu từ các attribute [MenuItem] và xoá sạch item động.
                AddMenuItemMethod.Invoke(null, new object[]
                {
                    menuPath, string.Empty, false, MenuPriority + index, execute, null
                });

                RegisteredPaths.Add(menuPath);
                index++;
            }
        }

        private static IEnumerable<string> CollectScenePaths()
        {
            if (IncludeAllProjectScenes)
            {
                return AssetDatabase.FindAssets("t:Scene", new[] { "Assets" })
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Where(p => !string.IsNullOrEmpty(p) && p.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                    .Distinct()
                    .OrderBy(p => Path.GetFileNameWithoutExtension(p), StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }

            return EditorBuildSettings.scenes
                .Where(s => s.enabled && !string.IsNullOrEmpty(s.path))
                .Select(s => s.path)
                .ToList();
        }

        private static void ClearRegistered()
        {
            if (RemoveMenuItemMethod == null)
            {
                RegisteredPaths.Clear();
                return;
            }

            for (int i = 0; i < RegisteredPaths.Count; i++)
                RemoveMenuItemMethod.Invoke(null, new object[] { RegisteredPaths[i] });

            RegisteredPaths.Clear();
        }

        private static void OpenScene(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (!File.Exists(path))
            {
                UnityEngine.Debug.LogWarning($"[Tidz] Scene không còn tồn tại: {path}");
                RequestRebuild();
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        }
    }

    internal sealed class SceneSwitcherAssetWatcher : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (ContainsScene(importedAssets)
                || ContainsScene(deletedAssets)
                || ContainsScene(movedAssets)
                || ContainsScene(movedFromAssetPaths))
            {
                SceneSwitcherMenu.RequestRebuild();
            }
        }

        private static bool ContainsScene(string[] paths)
        {
            if (paths == null)
                return false;

            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
#endif