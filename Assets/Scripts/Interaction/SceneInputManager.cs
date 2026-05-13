using Simonshouse.UI;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Simonshouse.Interaction
{
    /// <summary>
    /// Detecta clic y hover sobre <see cref="Interactable2D"/> vía física 2D.
    /// Colocar uno por escena de sala (Lobby, Estudio, etc.). Asignar layer <c>Interactable</c>.
    /// </summary>
    public class SceneInputManager : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private LayerMask interactableLayer;

        private Interactable2D _hovered;

        private void Awake()
        {
            if (sceneCamera == null)
                sceneCamera = Camera.main;

            if (interactableLayer.value == 0)
                interactableLayer = LayerMask.GetMask("Interactable");

            if (interactableLayer.value == 0)
                Debug.LogWarning("[SceneInputManager] Layer 'Interactable' no encontrado. Créalo en Tags & Layers y asigna el LayerMask.");
        }

        private void Update()
        {
            if (DialogManager.Instance != null && DialogManager.Instance.IsPlaying())
                return;

            if (HUDDialogPanel.Instance != null && HUDDialogPanel.Instance.IsObjectInteractionActive)
                return;

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (sceneCamera == null)
                return;

            Vector2 worldPos = GetMouseWorldPosition();
            UpdateHover(worldPos);

#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                TryClick(worldPos);
#else
            if (Input.GetMouseButtonDown(0))
                TryClick(worldPos);
#endif
        }

        private Vector2 GetMouseWorldPosition()
        {
            Vector3 screenPos;
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
                screenPos = Mouse.current.position.ReadValue();
            else
#endif
                screenPos = Input.mousePosition;

            float z = sceneCamera.orthographic
                ? Mathf.Abs(sceneCamera.transform.position.z)
                : sceneCamera.nearClipPlane;

            return sceneCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, z));
        }

        private void UpdateHover(Vector2 worldPos)
        {
            Collider2D col = Physics2D.OverlapPoint(worldPos, interactableLayer);
            var newHover = col != null ? col.GetComponent<Interactable2D>() : null;

            if (newHover != _hovered)
            {
                _hovered?.OnHoverExit();
                _hovered = newHover;
                _hovered?.OnHoverEnter();
            }
        }

        private void TryClick(Vector2 worldPos)
        {
            Collider2D col = Physics2D.OverlapPoint(worldPos, interactableLayer);
            if (col == null)
                return;

            Interactable2D interactable = col.GetComponent<Interactable2D>();
            interactable?.TryInteract();
        }
    }
}
