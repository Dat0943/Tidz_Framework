#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
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

            EditorApplication.delayCall += Rebuild;
            EditorBuildSettings.sceneListChanged += Rebuild;
        }

        private static void Rebuild()
        {
            if (AddMenuItemMethod == null)
                return;

            ClearRegistered();

            var scenes = EditorBuildSettings.scenes;
            var usedNames = new HashSet<string>();

            for (int i = 0; i < scenes.Length; i++)
            {
                var scene = scenes[i];
                if (!scene.enabled || string.IsNullOrEmpty(scene.path))
                    continue;

                string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                string uniqueName = sceneName;
                int dup = 1;

                while (!usedNames.Add(uniqueName))
                {
                    dup++;
                    uniqueName = $"{sceneName} ({dup})";
                }

                string menuPath = MenuRoot + uniqueName;
                string capturedPath = scene.path;

                Action execute = () => OpenScene(capturedPath);

                AddMenuItemMethod.Invoke(null, new object[]
                {
                    menuPath, string.Empty, false, MenuPriority + i, execute, null
                });

                RegisteredPaths.Add(menuPath);
            }

            EditorApplication.RepaintProjectWindow();
        }

        private static void ClearRegistered()
        {
            if (RemoveMenuItemMethod == null)
            {
                RegisteredPaths.Clear();
                return;
            }

            for (int i = 0; i < RegisteredPaths.Count; i++)
            {
                RemoveMenuItemMethod.Invoke(null, new object[] { RegisteredPaths[i] });
            }

            RegisteredPaths.Clear();
        }

        private static void OpenScene(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        }
    }
}
#endif
