using UnityEngine;

namespace Simonshouse.UI
{
    /// <summary>
    /// Solo para pruebas al abrir una escena de sala desde el editor sin pasar por MainMenu/Prologue.
    /// Instancia <see cref="GameManager"/> y <see cref="ChapterFlowManager"/> si faltan.
    /// </summary>
    public class SceneSafetyBootstrap : MonoBehaviour
    {
        [Tooltip("En builds standalone, el componente se destruye y no instancia prefabs (el flujo pasa por MainMenu).")]
        [SerializeField] private bool editorOnlyMode = true;

        [SerializeField] private GameObject gameManagerPrefab;
        [SerializeField] private GameObject chapterFlowPrefab;

        private void Awake()
        {
#if !UNITY_EDITOR
            if (editorOnlyMode)
            {
                Destroy(this);
                return;
            }
#endif
            if (GameManager.Instance == null)
            {
                if (gameManagerPrefab != null)
                    Instantiate(gameManagerPrefab);
                else
                {
                    var go = new GameObject("GameManager");
                    go.AddComponent<GameManager>();
                    go.AddComponent<AudioManager>();
                    Debug.LogWarning("[Bootstrap] GameManager creado en runtime (sin prefab asignado).");
                }
            }

            if (ChapterFlowManager.Instance == null)
            {
                if (chapterFlowPrefab != null)
                    Instantiate(chapterFlowPrefab);
                else
                {
                    var go = new GameObject("ChapterFlowManager");
                    go.AddComponent<ChapterFlowManager>();
                    Debug.LogWarning("[Bootstrap] ChapterFlowManager creado en runtime (sin prefab asignado).");
                }
            }
        }
    }
}
