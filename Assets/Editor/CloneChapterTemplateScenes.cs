using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Simonshouse.Editor
{
    /// <summary>
    /// Copia <c>Chapter1.unity</c> sobre Chapter2–5 y reemplaza el componente Chapter1Controller
    /// por el controlador del capítulo correspondiente (misma jerarquía UI que Chapter1).
    /// </summary>
    public static class CloneChapterTemplateScenes
    {
        private const string Chapter1Path = "Assets/Scenes/Chapter1.unity";

        private static readonly (string scene, string guid, string typeName)[] Targets =
        {
            ("Assets/Scenes/Chapter2.unity", "d2a3b4c5061728394a5b6c7d8e90f101", "Chapter2Controller"),
            ("Assets/Scenes/Chapter3.unity", "d3a3b4c5061728394a5b6c7d8e90f102", "Chapter3Controller"),
            ("Assets/Scenes/Chapter4.unity", "d4a3b4c5061728394a5b6c7d8e90f103", "Chapter4Controller"),
            ("Assets/Scenes/Chapter5.unity", "d5a3b4c5061728394a5b6c7d8e90f104", "Chapter5Controller"),
        };

        [MenuItem("Simonshouse/Clone Chapter1 Template → Chapters 2–5 (layout + controller)")]
        public static void CloneScenes()
        {
            if (!File.Exists(Chapter1Path))
            {
                EditorUtility.DisplayDialog("Clone chapters", "No se encuentra Chapter1.unity", "OK");
                return;
            }

            var ch1Guid = "e1f2a3b4c5d60718293a4b5c6d7e8091";

            foreach (var (scenePath, newGuid, typeName) in Targets)
            {
                File.Copy(Chapter1Path, scenePath, true);
                var yaml = File.ReadAllText(scenePath);
                yaml = yaml.Replace(
                    $"m_Script: {{fileID: 11500000, guid: {ch1Guid}, type: 3}}",
                    $"m_Script: {{fileID: 11500000, guid: {newGuid}, type: 3}}");
                yaml = yaml.Replace("Simonshouse.Chapters.Chapter1Controller",
                    $"Simonshouse.Chapters.{typeName}");
                yaml = yaml.Replace("m_Name: Chapter1Controller", $"m_Name: {typeName}");
                File.WriteAllText(scenePath, yaml);
                EditorSceneManager.OpenScene(scenePath);
                EditorSceneManager.SaveOpenScenes();
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Clone chapters",
                "Chapter2–5 sustituidos por copia de Chapter1 con controladores 2–5.", "OK");
        }
    }
}
