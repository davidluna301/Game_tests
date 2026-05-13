using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Simonshouse.UI
{
    /// <summary>
    /// Carga la escena HUD en modo additive solo sobre las salas de gameplay listadas.
    /// Se suscribe a <see cref="SceneManager.sceneLoaded"/> para cualquier transición.
    /// </summary>
    public static class HUDSceneOverlayLoader
    {
        private const string HudSceneName = "HUD";

        /// <summary>Solo estas escenas activan el HUD (carga additive).</summary>
        private static readonly HashSet<string> GameplayScenes = new()
        {
            "Lobby",
            "Estudio",
            "Habitacion",
            "Galeria",
            "Sotano"
        };

        private static bool initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (initialized)
                return;

            initialized = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
            ApplyForScene(SceneManager.GetActiveScene());
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Additive)
                return;

            ApplyForScene(scene);
        }

        private static void ApplyForScene(Scene scene)
        {
            if (scene.name == HudSceneName)
                return;

            bool isGameplay = GameplayScenes.Contains(scene.name);

            if (isGameplay)
                EnsureHudLoaded();
            else
                EnsureHudUnloaded();
        }

        private static void EnsureHudLoaded()
        {
            Scene hudScene = SceneManager.GetSceneByName(HudSceneName);
            if (hudScene.IsValid() && hudScene.isLoaded)
                return;

            SceneManager.LoadSceneAsync(HudSceneName, LoadSceneMode.Additive);
        }

        private static void EnsureHudUnloaded()
        {
            Scene hudScene = SceneManager.GetSceneByName(HudSceneName);
            if (!hudScene.IsValid() || !hudScene.isLoaded)
                return;

            SceneManager.UnloadSceneAsync(hudScene);
        }
    }
}
