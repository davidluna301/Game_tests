using Simonshouse.UI;
using UnityEngine;

namespace Simonshouse.Interaction
{
    /// <summary>Personaje en escena: abre cadena de diálogo en el HUD.</summary>
    public class CharacterInteractable : Interactable2D
    {
        [Header("Diálogo")]
        [SerializeField] private DialogChain onInteractDialog;

        protected override void OnInteract()
        {
            if (onInteractDialog == null || onInteractDialog.lines == null || onInteractDialog.lines.Count == 0)
            {
                Debug.LogWarning($"[CharacterInteractable:{interactableId}] Sin líneas de diálogo asignadas.");
                return;
            }

            if (DialogManager.Instance == null)
            {
                Debug.LogWarning("[CharacterInteractable] DialogManager no disponible.");
                return;
            }

            DialogManager.Instance.StartChain(onInteractDialog, null);
        }
    }
}
