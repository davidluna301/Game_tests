using UnityEngine;
using UnityEngine.SceneManagement;

namespace Simonshouse.Interaction
{
    /// <summary>Puerta / salida: carga otra escena al interactuar.</summary>
    public class DoorInteractable : Interactable2D
    {
        [Header("Puerta")]
        [SerializeField] private string targetSceneName;

        protected override void OnInteract()
        {
            if (string.IsNullOrEmpty(targetSceneName))
            {
                Debug.LogWarning($"[DoorInteractable:{interactableId}] targetSceneName vacío.");
                return;
            }

            SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
        }
    }
}
