using System.Linq;
using UnityEditor;

namespace Simonshouse.Editor
{
    public static class EnsureHudBuildSettings
    {
        private const string HudScenePath = "Assets/Scenes/HUD.unity";

        [InitializeOnLoadMethod]
        private static void EnsureHudSceneIncluded()
        {
            var scenes = EditorBuildSettings.scenes.ToList();
            bool exists = scenes.Any(scene => scene.path == HudScenePath);

            if (exists)
            {
                return;
            }

            scenes.Add(new EditorBuildSettingsScene(HudScenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
