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
            bool changed = false;

            if (!scenes.Any(scene => scene.path == HudScenePath))
            {
                scenes.Add(new EditorBuildSettingsScene(HudScenePath, true));
                changed = true;
            }

            if (changed)
            {
                EditorBuildSettings.scenes = scenes.ToArray();
            }
        }
    }
}
