using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simonshouse.UI
{
    /// <summary>Avanza cadenas de diálogo en el HUD (clic en continuar del panel de diálogo).</summary>
    public class DialogManager : MonoBehaviour
    {
        public static DialogManager Instance { get; private set; }

        private DialogChain _activeChain;
        private int _lineIndex = -1;
        private Action _onComplete;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        /// <summary>Inicia una cadena. Al terminar todas las líneas (clics), invoca <paramref name="onComplete"/>.</summary>
        public void StartChain(DialogChain chain, Action onComplete = null)
        {
            if (chain?.lines == null || chain.lines.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }

            _activeChain = chain;
            _lineIndex = -1;
            _onComplete = onComplete;
            AdvanceLine();
        }

        public void AdvanceLine()
        {
            if (_activeChain == null)
                return;

            _lineIndex++;

            while (_lineIndex < _activeChain.lines.Count)
            {
                var line = _activeChain.lines[_lineIndex];
                if (!LineRequirementsMet(line))
                {
                    _lineIndex++;
                    continue;
                }

                PresentLine(line);
                return;
            }

            FinishChain();
        }

        private static bool LineRequirementsMet(DialogLine line)
        {
            if (GameManager.Instance == null)
                return true;

            if (!string.IsNullOrEmpty(line.requiredClue) && !GameManager.Instance.HasClue(line.requiredClue))
                return false;
            if (!string.IsNullOrEmpty(line.requiredItem) && !GameManager.Instance.HasItem(line.requiredItem))
                return false;
            return true;
        }

        private void PresentLine(DialogLine line)
        {
            var hud = HUDDialogPanel.Instance;
            if (hud == null)
                return;

            var speaker = GetSpeakerLabel(line, _activeChain);
            hud.ShowDialog(speaker, line.content ?? "");

            if (!string.IsNullOrEmpty(_activeChain.characterName) && !line.isNarration)
                hud.ShowCharacter(_activeChain.characterName);
            else
                hud.SetCharacterVisible(false);

            hud.ShowContinueIcon();
        }

        private static string GetSpeakerLabel(DialogLine line, DialogChain chain)
        {
            if (line.isNarration)
                return "";
            if (!string.IsNullOrEmpty(line.speakerName))
                return line.speakerName;
            return chain.characterName ?? "";
        }

        private void FinishChain()
        {
            var cb = _onComplete;
            _activeChain = null;
            _onComplete = null;
            _lineIndex = -1;

            if (HUDDialogPanel.Instance != null)
            {
                HUDDialogPanel.Instance.SetDialogVisible(false);
                HUDDialogPanel.Instance.SetCharacterVisible(false);
            }

            cb?.Invoke();
        }
    }
}
