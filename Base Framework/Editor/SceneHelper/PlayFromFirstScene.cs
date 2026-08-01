#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Tidz.Editor
{
    public static class PlayFromFirstScene
    {
        private const string MenuPath = "Tidz/Play From First Scene";

        [MenuItem(MenuPath, priority = 100)]
        private static void Play()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            var scenes = EditorBuildSettings.scenes;
            string firstPath = null;

            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].enabled && !string.IsNullOrEmpty(scenes[i].path))
                {
                    firstPath = scenes[i].path;
                    break;
                }
            }

            if (string.IsNullOrEmpty(firstPath))
            {
                EditorUtility.DisplayDialog(
                    "Play From First Scene",
                    "Chưa có scene nào được enable trong Build Settings.",
                    "OK");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            EditorSceneManager.OpenScene(firstPath, OpenSceneMode.Single);
            EditorApplication.isPlaying = true;
        }
    }
}
#endif
