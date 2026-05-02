using UnityEngine;
using UnityEngine.SceneManagement;

namespace Simonshouse.UI
{
    public static class HUDSceneOverlayLoader
    {
        private const string HudSceneName = "HUD";

        // Scenes where HUD should stay hidden/unloaded.
        private static readonly string[] ExcludedScenes =
        {
            "New Game",
            "Chapter1"
        };

        private static bool initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
            UpdateHudStateFor(SceneManager.GetActiveScene());
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == HudSceneName)
            {
                return;
            }

            UpdateHudStateFor(scene);
        }

        private static void UpdateHudStateFor(Scene activeScene)
        {
            bool shouldHideHud = IsExcluded(activeScene.name);
            Scene hudScene = SceneManager.GetSceneByName(HudSceneName);
            bool hudLoaded = hudScene.IsValid() && hudScene.isLoaded;

            if (shouldHideHud)
            {
                if (hudLoaded)
                {
                    SceneManager.UnloadSceneAsync(HudSceneName);
                }

                return;
            }

            if (!hudLoaded)
            {
                SceneManager.LoadSceneAsync(HudSceneName, LoadSceneMode.Additive);
            }
        }

        private static bool IsExcluded(string sceneName)
        {
            for (int i = 0; i < ExcludedScenes.Length; i++)
            {
                if (ExcludedScenes[i] == sceneName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
