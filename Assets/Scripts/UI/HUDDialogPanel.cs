using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Simonshouse.UI
{
    /// Gestiona el panel de diálogo y el frame de sprite en el HUD.
    /// Complementa HUDInventoryToggle sin tocarlo.
    public class HUDDialogPanel : MonoBehaviour
    {
        public static HUDDialogPanel Instance { get; private set; }

        [Header("Panel de diálogo")]
        [SerializeField] private GameObject panelDialogBox;
        [SerializeField] private TextMeshProUGUI textSpeakerName;
        [SerializeField] private TextMeshProUGUI textDialogContent;
        [SerializeField] private GameObject iconContinue;

        [Header("Botón continuar")]
        [SerializeField] private Button btnContinue;

        [Header("Frame de sprite del personaje")]
        [SerializeField] private GameObject panelCharacterSprite;
        [SerializeField] private Image imgCharacterSprite;
        [SerializeField] private TextMeshProUGUI textCharacterName;

        private ItemData pendingItem;
        private bool isObjectMode;

        /// <summary>True mientras se muestra descripción de objeto (bloquear interacción de escena).</summary>
        public bool IsObjectInteractionActive => isObjectMode;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            SetDialogVisible(false);
            SetCharacterVisible(false);

            if (btnContinue != null)
            {
                btnContinue.onClick.RemoveAllListeners();
                btnContinue.onClick.AddListener(OnContinuePressed);
            }
        }

        private void OnDestroy()
        {
            if (btnContinue != null)
                btnContinue.onClick.RemoveListener(OnContinuePressed);
        }

        // ── Dialog Box ────────────────────────────────────────
        public void ShowDialog(string speaker, string content)
        {
            isObjectMode = false;

            if (panelDialogBox == null) return;
            panelDialogBox.SetActive(true);
            if (iconContinue != null) iconContinue.SetActive(false);
            if (textSpeakerName != null) textSpeakerName.text = string.IsNullOrEmpty(speaker) ? "" : speaker.ToUpper();
            if (textDialogContent != null) textDialogContent.text = content;
        }

        public void ShowContinueIcon()
        {
            if (iconContinue != null) iconContinue.SetActive(true);
        }

        public void SetDialogVisible(bool visible)
        {
            if (panelDialogBox != null) panelDialogBox.SetActive(visible);
        }

        public void HideDialog() => SetDialogVisible(false);

        /// <summary>Descripción de objeto: sin sprite de personaje; Continuar añade ítem pendiente.</summary>
        public void ShowObjectDescription(string title, string description)
        {
            isObjectMode = true;
            pendingItem = null;

            SetCharacterVisible(false);

            if (panelDialogBox != null) panelDialogBox.SetActive(true);
            if (iconContinue != null) iconContinue.SetActive(true);
            if (textSpeakerName != null)
                textSpeakerName.text = string.IsNullOrEmpty(title) ? "" : title.ToUpper();
            if (textDialogContent != null) textDialogContent.text = description ?? "";
        }

        public void SetPendingItem(ItemData item) => pendingItem = item;

        /// <summary>Enlazar también desde Btn_Continue en Inspector si no se usa el cableado en <see cref="Awake"/>.</summary>
        public void OnContinuePressed()
        {
            if (isObjectMode)
            {
                if (pendingItem != null)
                {
                    GameManager.Instance?.AddItem(pendingItem);
                    pendingItem = null;
                }

                SetDialogVisible(false);
                isObjectMode = false;
            }
            else
            {
                DialogManager.Instance?.AdvanceLine();
            }
        }

        // ── Sprite del personaje ──────────────────────────────
        /// sprite puede ser null (sin asignar en postproducción).
        public void ShowCharacter(string charName, Sprite sprite = null)
        {
            if (panelCharacterSprite == null) return;
            panelCharacterSprite.SetActive(true);
            if (textCharacterName != null) textCharacterName.text = charName.ToUpper();
            if (sprite != null && imgCharacterSprite != null)
                imgCharacterSprite.sprite = sprite;
        }

        public void SetCharacterVisible(bool visible)
        {
            if (panelCharacterSprite != null) panelCharacterSprite.SetActive(visible);
        }
    }
}
