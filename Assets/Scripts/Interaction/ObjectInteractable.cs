using Simonshouse.Interaction;
using UnityEngine;

namespace Simonshouse.UI
{
    /// <summary>Objeto del escenario: muestra descripción en el HUD y opcionalmente guarda un ítem al pulsar Continuar.</summary>
    public class ObjectInteractable : Interactable2D
    {
        [Header("Contenido del objeto")]
        [SerializeField] private string objectTitle;
        [SerializeField, TextArea(2, 8)] private string objectDescription;

        [Header("Ítem asociado (opcional)")]
        [SerializeField] private ItemData associatedItem;
        [SerializeField] private bool addToInventory = true;

        [Header("Pista asociada (opcional)")]
        [SerializeField] private string clueToAdd;

        protected override void OnInteract()
        {
            HUDDialogPanel.Instance?.ShowObjectDescription(objectTitle, objectDescription);

            if (!string.IsNullOrEmpty(clueToAdd))
                GameManager.Instance?.AddClue(clueToAdd);

            if (associatedItem != null && addToInventory)
                HUDDialogPanel.Instance?.SetPendingItem(associatedItem);
        }

        protected override void OnAlreadyUsed()
        {
            HUDDialogPanel.Instance?.ShowObjectDescription(objectTitle,
                $"[Ya examinado]\n{objectDescription}");
        }
    }
}
