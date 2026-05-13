using Simonshouse.Interaction;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Simonshouse.UI
{
    /// <summary>Puerta: carga escena destino. Condición opcional por pista o ítem. v2.9: hook opcional al Lobby.</summary>
    public class DoorInteractable : Interactable2D
    {
        [Header("Destino")]
        [SerializeField] private string targetSceneName;

        [Header("Condición de acceso (opcional)")]
        [SerializeField] private string requiredClue;
        [SerializeField] private string requiredItem;

        [Header("Mensajes")]
        [SerializeField, TextArea(1, 3)]
        private string lockedMessage = "La puerta está cerrada.";

        [Header("Hook de sala (opcional)")]
        [SerializeField] private LobbyController lobbyHook;

        protected override void Awake()
        {
            singleUse = false;
            base.Awake();
        }

        protected override void OnInteract()
        {
            if (!CheckConditions())
            {
                HUDDialogPanel.Instance?.ShowObjectDescription("PUERTA BLOQUEADA", lockedMessage);
                return;
            }

            LoadScene();
        }

        private bool CheckConditions()
        {
            if (!string.IsNullOrEmpty(requiredClue) &&
                !(GameManager.Instance?.HasClue(requiredClue) ?? false))
                return false;

            if (!string.IsNullOrEmpty(requiredItem) &&
                !(GameManager.Instance?.HasItem(requiredItem) ?? false))
                return false;

            return true;
        }

        private void LoadScene()
        {
            if (lobbyHook != null && !lobbyHook.OnExitingRoom())
                return;

            if (string.IsNullOrEmpty(targetSceneName))
            {
                Debug.LogWarning($"[Door] targetSceneName vacío en {gameObject.name}");
                return;
            }

            SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
        }
    }
}
