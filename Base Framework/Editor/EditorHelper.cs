#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tidz.Editor
{
    public static class EditorHelper
    {
        [MenuItem("Tidz/Clear Device Data")]
        private static void ClearDeviceData()
        {
            LDataBlockHelper.ClearDeviceData();
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Tidz/Create Audio Config File")]
        public static void OpenCreateAudioConfigWindow()
        {
            CreateAudioConfigWindow.Open();
        }
    }

    public sealed class CreateAudioConfigWindow : EditorWindow
    {
        private DefaultAsset targetFolder;
        private readonly List<AudioClip> clips = new();

        private Vector2 scrollPos;

        public static void Open()
        {
            var window = GetWindow<CreateAudioConfigWindow>("Create Audio Config");
            window.minSize = new Vector2(500f, 420f);
            window.Show();
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawFolderField();
            EditorGUILayout.Space(8);
            DrawDropArea();
            EditorGUILayout.Space(8);
            DrawClipList();
            EditorGUILayout.Space(8);
            DrawBottomButtons();
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField("Create Audio Config", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Kéo thả AudioClip vào đây hoặc thêm thủ công. Chọn folder để lưu các file AudioConfig rồi bấm Create.",
                MessageType.Info);
        }

        private void DrawFolderField()
        {
            EditorGUILayout.LabelField("Target Folder", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            targetFolder = (DefaultAsset)EditorGUILayout.ObjectField(
                "Folder",
                targetFolder,
                typeof(DefaultAsset),
                false);

            if (EditorGUI.EndChangeCheck() && targetFolder != null)
            {
                string path = AssetDatabase.GetAssetPath(targetFolder);
                if (!AssetDatabase.IsValidFolder(path))
                {
                    Debug.LogWarning("Object được chọn không phải folder hợp lệ.");
                    targetFolder = null;
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Select Folder", GUILayout.Width(120)))
                {
                    string selectedPath = EditorUtility.OpenFolderPanel("Select Target Folder", Application.dataPath, "");

                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        if (selectedPath.StartsWith(Application.dataPath))
                        {
                            string relativePath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                            targetFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(relativePath);
                        }
                        else
                        {
                            EditorUtility.DisplayDialog(
                                "Folder không hợp lệ",
                                "Vui lòng chọn folder nằm bên trong thư mục Assets của project.",
                                "OK");
                        }
                    }
                }
            }
        }

        private void DrawDropArea()
        {
            EditorGUILayout.LabelField("Audio Clips", EditorStyles.boldLabel);

            Rect dropArea = GUILayoutUtility.GetRect(0f, 80f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Kéo thả AudioClip vào đây", EditorStyles.helpBox);

            Event evt = Event.current;

            if (dropArea.Contains(evt.mousePosition))
            {
                switch (evt.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        {
                            bool hasAudio = false;

                            foreach (Object dragged in DragAndDrop.objectReferences)
                            {
                                if (dragged is AudioClip)
                                {
                                    hasAudio = true;
                                    break;
                                }
                            }

                            if (hasAudio)
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                                if (evt.type == EventType.DragPerform)
                                {
                                    DragAndDrop.AcceptDrag();

                                    foreach (Object dragged in DragAndDrop.objectReferences)
                                    {
                                        if (dragged is AudioClip clip)
                                            AddClip(clip);
                                    }
                                }

                                evt.Use();
                            }

                            break;
                        }
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add Selected AudioClips"))
                {
                    foreach (Object obj in Selection.objects)
                    {
                        if (obj is AudioClip clip)
                            AddClip(clip);
                    }
                }

                if (GUILayout.Button("Clear List"))
                {
                    clips.Clear();
                }
            }
        }

        private void DrawClipList()
        {
            EditorGUILayout.LabelField($"Clip List ({clips.Count})", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(180));

            for (int i = 0; i < clips.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    clips[i] = (AudioClip)EditorGUILayout.ObjectField(
                        $"Clip {i + 1}",
                        clips[i],
                        typeof(AudioClip),
                        false);

                    if (GUILayout.Button("X", GUILayout.Width(28)))
                    {
                        clips.RemoveAt(i);
                        i--;
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawBottomButtons()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                GUI.enabled = targetFolder != null && clips.Count > 0;

                if (GUILayout.Button("Create Audio Configs", GUILayout.Width(180), GUILayout.Height(30)))
                {
                    CreateConfigs();
                }

                GUI.enabled = true;
            }
        }

        private void AddClip(AudioClip clip)
        {
            if (clip == null)
                return;

            if (!clips.Contains(clip))
                clips.Add(clip);
        }

        private void CreateConfigs()
        {
            string folderPath = AssetDatabase.GetAssetPath(targetFolder);

            if (string.IsNullOrEmpty(folderPath) || !AssetDatabase.IsValidFolder(folderPath))
            {
                EditorUtility.DisplayDialog("Lỗi", "Folder lưu file không hợp lệ.", "OK");
                return;
            }

            int createdCount = 0;
            int skippedCount = 0;

            try
            {
                for (int i = 0; i < clips.Count; i++)
                {
                    AudioClip clip = clips[i];

                    if (clip == null)
                    {
                        skippedCount++;
                        continue;
                    }

                    string assetPath = AssetDatabase.GenerateUniqueAssetPath(
                        Path.Combine(folderPath, clip.name + ".asset"));

                    AudioConfig config = ScriptableObject.CreateInstance<AudioConfig>();
                    if (config == null)
                    {
                        skippedCount++;
                        continue;
                    }

                    config.Construct(clip);

                    AssetDatabase.CreateAsset(config, assetPath);
                    EditorUtility.SetDirty(config);

                    createdCount++;
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                EditorUtility.DisplayDialog("Lỗi", ex.Message, "OK");
                return;
            }

            EditorUtility.DisplayDialog(
                "Hoàn tất",
                $"Đã tạo: {createdCount}\nBỏ qua: {skippedCount}",
                "OK");
        }
    }
}
#endif