using Simonshouse.UI;
using UnityEngine;

namespace Simonshouse.Interaction
{
    /// <summary>Zona 2D que abre el panel de decisión del Lobby (v2.9).</summary>
    public class SceneTriggerInteractable : Interactable2D
    {
        [SerializeField] private LobbyController controller;

        protected override void OnInteract()
        {
            controller?.OpenDecisionPanel();
        }
    }
}
