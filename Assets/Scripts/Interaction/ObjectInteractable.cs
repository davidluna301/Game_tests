using Simonshouse.UI;
using UnityEngine;

namespace Simonshouse.Interaction
{
    /// <summary>Objeto examinable: descripción (log / futuro HUD) y opcionalmente otorga ítem.</summary>
    public class ObjectInteractable : Interactable2D
    {
        [Header("Objeto")]
        [SerializeField, TextArea(2, 8)] private string inspectDescription = "Un objeto que aún no tiene descripción final.";
        [SerializeField] private string grantItemId;
        [SerializeField] private string grantItemDisplayName;

        protected override void OnInteract()
        {
            if (!string.IsNullOrEmpty(inspectDescription))
                Debug.Log($"[Interactable:{interactableId}] {inspectDescription}");

            if (GameManager.Instance != null && !string.IsNullOrEmpty(grantItemId))
                GameManager.Instance.GrantItem(grantItemId, grantItemDisplayName);
        }
    }
}
