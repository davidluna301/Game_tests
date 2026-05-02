using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Simonshouse.UI
{
    public class HUDInventoryToggle : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject inventarioPanel;
        [SerializeField] private Button closeButton;

        [Header("Input")]
        [SerializeField] private KeyCode toggleKey = KeyCode.I;

        private void Awake()
        {
            if (inventarioPanel == null)
            {
                Transform t = transform.Find("Inventario");
                if (t != null)
                {
                    inventarioPanel = t.gameObject;
                }
            }

            if (closeButton == null && inventarioPanel != null)
            {
                Transform t = inventarioPanel.transform.Find("BtnCerrar");
                if (t != null)
                {
                    closeButton = t.GetComponent<Button>();
                }
            }

            if (inventarioPanel != null)
            {
                inventarioPanel.SetActive(false);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseInventory);
            }
        }

        private void OnDestroy()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(CloseInventory);
            }
        }

        private void Update()
        {
            if (WasTogglePressedThisFrame())
            {
                ToggleInventory();
            }
        }

        public void ToggleInventory()
        {
            if (inventarioPanel == null)
            {
                return;
            }

            inventarioPanel.SetActive(!inventarioPanel.activeSelf);
        }

        public void OpenInventory()
        {
            if (inventarioPanel == null)
            {
                return;
            }

            inventarioPanel.SetActive(true);
        }

        public void CloseInventory()
        {
            if (inventarioPanel == null)
            {
                return;
            }

            inventarioPanel.SetActive(false);
        }

        private bool WasTogglePressedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
            {
                return true;
            }
#endif
            return Input.GetKeyDown(toggleKey);
        }
    }
}
