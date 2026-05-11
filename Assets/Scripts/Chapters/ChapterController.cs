using Simonshouse.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Simonshouse.Chapters
{
    public abstract class ChapterController : MonoBehaviour
    {
        [Header("Narrativa del capítulo")]
        [SerializeField, TextArea(4, 14)] protected string narrativeText;
        [SerializeField] protected string chapterTitle;

        [Header("Paneles de UI")]
        [SerializeField] protected GameObject panelNarrativeIntro;
        [SerializeField] protected GameObject panelExploration;
        [SerializeField] protected GameObject panelInteractions;
        [SerializeField] protected GameObject panelDecision;

        [Header("Contenedor de opciones (compartido)")]
        [SerializeField] protected Transform optionsContainer;
        [SerializeField] protected GameObject optionButtonPrefab;

        [Header("Títulos de UI")]
        [SerializeField] protected TextMeshProUGUI textChapterTitle;
        [SerializeField] protected TextMeshProUGUI textNarrativeBox;

        protected virtual void Start()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetChapter(GetChapterNumber());
            HUDManager.Instance?.ShowIsolationHUD();

            SetPanelState(showNarrative: true, showExploration: false,
                showInteractions: false, showDecision: false);

            if (textChapterTitle != null) textChapterTitle.text = chapterTitle;
            if (textNarrativeBox != null) textNarrativeBox.text = GetNarrativeText();
        }

        protected void SetPanelState(bool showNarrative, bool showExploration,
            bool showInteractions, bool showDecision)
        {
            if (panelNarrativeIntro != null) panelNarrativeIntro.SetActive(showNarrative);
            if (panelExploration != null) panelExploration.SetActive(showExploration);
            if (panelInteractions != null) panelInteractions.SetActive(showInteractions);
            if (panelDecision != null) panelDecision.SetActive(showDecision);

            if (optionsContainer != null)
                optionsContainer.gameObject.SetActive(showExploration || showInteractions || showDecision);
        }

        public void OnNarrativeContinue()
        {
            SetPanelState(false, true, false, false);
            LoadExploration();
        }

        protected void GoToInteractions()
        {
            SetPanelState(false, false, true, false);
            LoadInteractions();
        }

        protected void GoToDecision()
        {
            SetPanelState(false, false, false, true);
            LoadDecision();
        }

        protected void SpawnOptionButton(string label, System.Action onClick)
        {
            if (optionButtonPrefab == null || optionsContainer == null) return;
            var btn = Instantiate(optionButtonPrefab, optionsContainer);
            var lbl = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (lbl != null) lbl.text = label;
            var button = btn.GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(() => onClick?.Invoke());
        }

        protected void ClearOptions()
        {
            if (optionsContainer == null) return;
            foreach (Transform child in optionsContainer)
                Destroy(child.gameObject);
        }

        protected void EndChapter()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.EndChapter();
            ChapterFlowManager.Instance?.HandleChapterEnd();
        }

        protected abstract int GetChapterNumber();
        protected virtual string GetNarrativeText() => narrativeText;
        protected abstract void LoadExploration();
        protected abstract void LoadInteractions();
        protected abstract void LoadDecision();
    }
}
