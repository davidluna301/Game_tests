using UnityEngine;
using UnityEngine.SceneManagement;

namespace Simonshouse.UI
{
    public class ChapterFlowManager : MonoBehaviour
    {
        public static ChapterFlowManager Instance { get; private set; }

        private string pendingDeathCharacter;
        private string nextSceneName = "";

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            TrySubscribeGameManager();
        }

        private void Start()
        {
            TrySubscribeGameManager();
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnCharacterDied -= OnCharacterDied;
        }

        private void TrySubscribeGameManager()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.OnCharacterDied -= OnCharacterDied;
            GameManager.Instance.OnCharacterDied += OnCharacterDied;
        }

        private void OnCharacterDied(string name)
        {
            pendingDeathCharacter = name;
            Debug.Log($"[Flow] Muerte pendiente: {name}");
        }

        /// Llamado al final de cada capítulo por el ChapterController.
        public void HandleChapterEnd()
        {
            if (GameManager.Instance == null) return;

            int current = GameManager.Instance.CurrentChapter;
            nextSceneName = current < 5 ? $"Chapter{current + 1}" : "Endings";

            HUDManager.Instance?.RefreshIsolationBars();

            if (!string.IsNullOrEmpty(pendingDeathCharacter))
            {
                Debug.Log("[Flow] Cargando DeathScreen en modo Additive.");
                SceneManager.LoadScene("DeathScreen", LoadSceneMode.Additive);
            }
            else
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }

        public string GetPendingDeathCharacter() => pendingDeathCharacter;

        /// Llamado por DeathScreenController cuando la cinemática termina.
        public void OnDeathScreenFinished()
        {
            pendingDeathCharacter = null;
            SceneManager.UnloadSceneAsync("DeathScreen");
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
