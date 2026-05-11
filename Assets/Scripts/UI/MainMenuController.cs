using Simonshouse.Endings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Simonshouse.UI
{
    public class MainMenuController : MonoBehaviour
    {
        private void Awake()
        {
            var canvas = transform.Find("Canvas");
            if (canvas == null) return;
            WireButton(canvas, "Btn_NewGame", StartNewGame);
            WireButton(canvas, "Btn_Continue", ContinueGame);
            WireButton(canvas, "Btn_Quit", QuitGame);
        }

        private static void WireButton(Transform parent, string childName, UnityEngine.Events.UnityAction handler)
        {
            var t = parent.Find(childName);
            if (t == null) return;
            var btn = t.GetComponent<Button>();
            if (btn == null) return;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(handler);
        }

        public void StartNewGame()
        {
            PostCreditsController.EnteredFromFinalA = false;
            GameManager.Instance?.ResetRunState();
            ChapterFlowManager.Instance?.ClearForNewRun();
            SceneManager.LoadScene("Prologue");
        }

        public void ContinueGame()
        {
            // TODO v2.0: sistema de guardado
            Debug.Log("[Menu] Continuar — pendiente de implementación");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
