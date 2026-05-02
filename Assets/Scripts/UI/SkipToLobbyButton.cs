using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Simonshouse.UI
{
    [RequireComponent(typeof(Button))]
    public class SkipToLobbyButton : MonoBehaviour
    {
        [SerializeField] private string lobbySceneName = "Lobby";

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(SkipToLobby);
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(SkipToLobby);
        }

        private void SkipToLobby()
        {
            SceneManager.LoadScene(lobbySceneName);
        }
    }
}
