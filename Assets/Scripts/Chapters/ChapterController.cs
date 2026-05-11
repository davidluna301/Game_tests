using System;
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

        [Header("Contenedor de opciones (compartido o por fase)")]
        [SerializeField] protected Transform optionsContainer;
        [SerializeField] protected Transform explorationOptionsContainer;
        [SerializeField] protected Transform interactionsOptionsContainer;
        [SerializeField] protected Transform decisionOptionsContainer;
        [SerializeField] protected GameObject optionButtonPrefab;

        [Header("Títulos de UI")]
        [SerializeField] protected TextMeshProUGUI textChapterTitle;
        [SerializeField] protected TextMeshProUGUI textNarrativeBox;

        private Transform _optionsTarget;

        protected Transform OptionsTarget => _optionsTarget != null ? _optionsTarget : optionsContainer;

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

            if (explorationOptionsContainer != null && interactionsOptionsContainer != null &&
                decisionOptionsContainer != null)
            {
                explorationOptionsContainer.gameObject.SetActive(showExploration);
                interactionsOptionsContainer.gameObject.SetActive(showInteractions);
                decisionOptionsContainer.gameObject.SetActive(showDecision);
            }
            else if (optionsContainer != null)
            {
                optionsContainer.gameObject.SetActive(showExploration || showInteractions || showDecision);
            }
        }

        public void OnNarrativeContinue()
        {
            SetPanelState(false, true, false, false);
            PrepareExplorationOptions();
            LoadExploration();
        }

        protected void PrepareExplorationOptions()
        {
            _optionsTarget = explorationOptionsContainer != null ? explorationOptionsContainer : optionsContainer;
        }

        protected void PrepareInteractionsOptions()
        {
            _optionsTarget = interactionsOptionsContainer != null ? interactionsOptionsContainer : optionsContainer;
        }

        protected void PrepareDecisionOptions()
        {
            _optionsTarget = decisionOptionsContainer != null ? decisionOptionsContainer : optionsContainer;
        }

        protected void GoToInteractions()
        {
            SetPanelState(false, false, true, false);
            PrepareInteractionsOptions();
            LoadInteractions();
        }

        protected void GoToDecision()
        {
            SetPanelState(false, false, false, true);
            PrepareDecisionOptions();
            LoadDecision();
        }

        protected void SpawnOptionButton(string label, System.Action onClick)
        {
            if (optionButtonPrefab == null || OptionsTarget == null) return;
            var btn = Instantiate(optionButtonPrefab, OptionsTarget);
            var lbl = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (lbl != null) lbl.text = label;
            var button = btn.GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(() => onClick?.Invoke());
        }

        protected void ClearOptions()
        {
            var t = OptionsTarget;
            if (t == null) return;
            foreach (Transform child in t)
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

        protected static void RunDialog(DialogChain chain, Action onComplete)
        {
            if (DialogManager.Instance != null)
                DialogManager.Instance.StartChain(chain, onComplete);
            else
                onComplete?.Invoke();
        }
    }
}
