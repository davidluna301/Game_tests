using System.Collections.Generic;
using Simonshouse.Interaction;
using UnityEngine;

namespace Simonshouse.UI
{
    /// <summary>Personaje en escena: sprite en el HUD y cadena de diálogo al hacer clic.</summary>
    public class CharacterInteractable : Interactable2D
    {
        [Header("Datos del personaje")]
        [SerializeField] private string characterName;
        [SerializeField] private Sprite characterSprite;

        [Header("Cadena de diálogo")]
        [SerializeField] private List<DialogLineData> dialogLines = new();

        protected override void OnInteract()
        {
            if (string.IsNullOrEmpty(characterName))
                return;

            if (GameManager.Instance != null &&
                GameManager.Instance.Characters.TryGetValue(characterName, out var c) &&
                !c.IsAlive)
                return;

            HUDDialogPanel.Instance?.ShowCharacter(characterName, characterSprite);

            var chain = BuildChain();
            if (chain.lines == null || chain.lines.Count == 0)
            {
                Debug.LogWarning($"[CharacterInteractable:{interactableId}] Sin líneas de diálogo.");
                return;
            }

            DialogManager.Instance?.StartChain(chain);
        }

        protected override void OnAlreadyUsed()
        {
            OnInteract();
        }

        private DialogChain BuildChain()
        {
            var lines = new List<DialogLine>();
            if (dialogLines == null)
                return new DialogChain { characterName = characterName, lines = lines };

            foreach (var d in dialogLines)
            {
                if (d == null)
                    continue;
                lines.Add(new DialogLine
                {
                    speakerName = d.isNarration ? "" : characterName.ToUpper(),
                    content = d.content,
                    isNarration = d.isNarration,
                    requiredClue = d.requiredClue,
                    requiredItem = d.requiredItem
                });
            }

            return new DialogChain
            {
                chainId = $"C1_{characterName}",
                characterName = characterName,
                lines = lines
            };
        }
    }

    /// <summary>Línea de diálogo configurable en el Inspector (personajes en escena).</summary>
    [System.Serializable]
    public class DialogLineData
    {
        [TextArea(2, 5)]
        public string content;
        public bool isNarration;
        public string requiredClue;
        public string requiredItem;
    }
}
