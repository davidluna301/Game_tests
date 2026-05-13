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

        [Header("Efectos sobre personajes (opcional)")]
        [Tooltip("Nombre clave en GameManager.Characters, ej. Robert")]
        [SerializeField] private string characterIsolationEffect;
        [Tooltip("Positivo sube aislamiento; negativo lo baja.")]
        [SerializeField] private int isolationAmount;

        protected override void OnInteract()
        {
            HUDDialogPanel.Instance?.ShowObjectDescription(objectTitle, objectDescription);

            if (!string.IsNullOrEmpty(clueToAdd))
                GameManager.Instance?.AddClue(clueToAdd);

            if (associatedItem != null && addToInventory)
                HUDDialogPanel.Instance?.SetPendingItem(associatedItem);

            if (!string.IsNullOrEmpty(characterIsolationEffect) &&
                GameManager.Instance != null &&
                GameManager.Instance.Characters.TryGetValue(characterIsolationEffect, out var c))
            {
                c.AddIsolation(isolationAmount);
                Debug.Log($"[Object] {characterIsolationEffect} isolation {(isolationAmount >= 0 ? "+" : "")}{isolationAmount}");
            }
        }

        protected override void OnAlreadyUsed()
        {
            HUDDialogPanel.Instance?.ShowObjectDescription(objectTitle,
                $"[Ya examinado]\n{objectDescription}");
        }
    }
}
