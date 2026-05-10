using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Simonshouse.UI
{
    [RequireComponent(typeof(Button))]
    public class DoorButtonLoader : MonoBehaviour
    {
        [SerializeField] private string targetSceneName;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.RemoveListener(LoadTargetScene);
            button.onClick.AddListener(LoadTargetScene);
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(LoadTargetScene);
            }
        }

        private void LoadTargetScene()
        {
            if (string.IsNullOrEmpty(targetSceneName))
            {
                return;
            }

            SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
        }
    }
}

