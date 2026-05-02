using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Simonshouse.UI
{
    [RequireComponent(typeof(Button))]
    public class NewGameButtonLoader : MonoBehaviour
    {
        [SerializeField] private string chapterSceneName = "Chapter1";

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.RemoveListener(LoadChapter);
            button.onClick.AddListener(LoadChapter);
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(LoadChapter);
            }
        }

        private void LoadChapter()
        {
            SceneManager.LoadScene(chapterSceneName);
        }
    }
}
