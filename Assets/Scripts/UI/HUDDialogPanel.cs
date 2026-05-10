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
        [SerializeField] private GameObject      panelDialogBox;
        [SerializeField] private TextMeshProUGUI textSpeakerName;
        [SerializeField] private TextMeshProUGUI textDialogContent;
        [SerializeField] private GameObject      iconContinue;

        [Header("Frame de sprite del personaje")]
        [SerializeField] private GameObject      panelCharacterSprite;
        [SerializeField] private Image           imgCharacterSprite;
        [SerializeField] private TextMeshProUGUI textCharacterName;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            SetDialogVisible(false);
            SetCharacterVisible(false);
        }

        // ── Dialog Box ────────────────────────────────────────
        public void ShowDialog(string speaker, string content)
        {
            if (panelDialogBox   == null) return;
            panelDialogBox.SetActive(true);
            if (iconContinue     != null) iconContinue.SetActive(false);
            if (textSpeakerName  != null) textSpeakerName.text  = speaker.ToUpper();
            if (textDialogContent!= null) textDialogContent.text = content;
        }

        public void ShowContinueIcon()
        {
            if (iconContinue != null) iconContinue.SetActive(true);
        }

        public void SetDialogVisible(bool visible)
        {
            if (panelDialogBox != null) panelDialogBox.SetActive(visible);
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
