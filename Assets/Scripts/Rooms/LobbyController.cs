using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Simonshouse.UI
{
    /// <summary>v2.9 — Lobby: panel de decisión grupal, modificadores de aislamiento y cierre de sala por puertas.</summary>
    public class LobbyController : MonoBehaviour
    {
        [Header("Panel de decisión")]
        [SerializeField] private GameObject panelDecision;
        [SerializeField] private TextMeshProUGUI textDecisionPrompt;
        [SerializeField] private Transform decisionButtonsContainer;
        [SerializeField] private GameObject decisionButtonPrefab;

        [Header("Botón para abrir el panel de decisión")]
        [SerializeField] private GameObject btnShowDecision;

        private bool decisionMade;

        private void Start()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.EnterRoom("Lobby");

            HUDManager.Instance?.ShowIsolationHUD();
            HUDManager.Instance?.RefreshIsolationBars();

            if (panelDecision != null)
                panelDecision.SetActive(false);
            if (btnShowDecision != null)
                btnShowDecision.SetActive(true);

            var openBtn = btnShowDecision != null ? btnShowDecision.GetComponent<Button>() : null;
            if (openBtn != null)
            {
                openBtn.onClick.RemoveListener(OpenDecisionPanel);
                openBtn.onClick.AddListener(OpenDecisionPanel);
            }

            if (GameManager.Instance != null)
                GameManager.Instance.OnCharacterDied += OnCharacterDied;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnCharacterDied -= OnCharacterDied;

            var openBtn = btnShowDecision != null ? btnShowDecision.GetComponent<Button>() : null;
            if (openBtn != null)
                openBtn.onClick.RemoveListener(OpenDecisionPanel);
        }

        // ── Panel de decisión ─────────────────────────────────
        public void OpenDecisionPanel()
        {
            if (decisionMade)
                return;
            if (panelDecision == null)
                return;

            panelDecision.SetActive(true);
            BuildDecisionButtons();

            if (textDecisionPrompt != null)
            {
                textDecisionPrompt.text =
                    "Llevan una hora en el lobby. La tensión es palpable. " +
                    "Nadie ha dicho por qué está realmente aquí.\n\n" +
                    "El silencio empieza a pesar.";
            }
        }

        private void BuildDecisionButtons()
        {
            if (decisionButtonsContainer == null)
                return;

            foreach (Transform child in decisionButtonsContainer)
                Destroy(child.gameObject);

            if (decisionButtonPrefab != null)
            {
                SpawnDecisionButton("[1] Proponer explorar la mansión juntos", () => ApplyDecision(1));
                SpawnDecisionButton("[2] Sugerir que cada uno explore por su cuenta", () => ApplyDecision(2));
                SpawnDecisionButton("[3] Intentar que el grupo hable abiertamente sobre Simón", () => ApplyDecision(3));
            }
            else
            {
                SpawnRuntimeButton("[1] Proponer explorar la mansión juntos", () => ApplyDecision(1));
                SpawnRuntimeButton("[2] Sugerir que cada uno explore por su cuenta", () => ApplyDecision(2));
                SpawnRuntimeButton("[3] Intentar que el grupo hable abiertamente sobre Simón", () => ApplyDecision(3));
            }
        }

        private void SpawnDecisionButton(string label, System.Action onClick)
        {
            var btn = Instantiate(decisionButtonPrefab, decisionButtonsContainer);
            var lbl = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (lbl != null)
                lbl.text = label;
            var uiBtn = btn.GetComponent<Button>();
            if (uiBtn == null)
                uiBtn = btn.GetComponentInChildren<Button>();
            if (uiBtn == null)
            {
                Debug.LogWarning("[LobbyController] decisionButtonPrefab sin Button.");
                return;
            }

            uiBtn.onClick.AddListener(() =>
            {
                onClick?.Invoke();
                if (panelDecision != null)
                    panelDecision.SetActive(false);
            });
        }

        /// <summary>Botones de decisión sin prefab (fallback en runtime).</summary>
        private void SpawnRuntimeButton(string label, System.Action onClick)
        {
            var go = new GameObject("Btn_DecisionRuntime");
            go.transform.SetParent(decisionButtonsContainer, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(900, 56);
            var img = go.AddComponent<Image>();
            img.color = new Color(0.22f, 0.22f, 0.28f, 1f);
            var uiBtn = go.AddComponent<Button>();
            uiBtn.targetGraphic = img;

            var child = new GameObject("Text");
            child.transform.SetParent(go.transform, false);
            var crt = child.AddComponent<RectTransform>();
            crt.anchorMin = Vector2.zero;
            crt.anchorMax = Vector2.one;
            crt.offsetMin = Vector2.zero;
            crt.offsetMax = Vector2.zero;
            child.AddComponent<CanvasRenderer>();
            var tmp = child.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 22;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;

            uiBtn.onClick.AddListener(() =>
            {
                onClick?.Invoke();
                if (panelDecision != null)
                    panelDecision.SetActive(false);
            });
        }

        private void ApplyDecision(int option)
        {
            var alive = GameManager.Instance?.GetAliveCharacters() ?? new List<string>();
            string narrativeResult;

            switch (option)
            {
                case 1:
                    GameManager.Instance?.AddDecision("grupo_unido_c1");
                    foreach (var n in alive)
                    {
                        if (GameManager.Instance?.Characters != null &&
                            GameManager.Instance.Characters.TryGetValue(n, out var ch))
                            ch.Connect(10);
                    }

                    narrativeResult =
                        "El grupo acepta, aunque con reservas. Hay algo reconfortante " +
                        "en moverse juntos por una casa que ninguno conoce del todo. " +
                        "Los pasos de cinco personas suenan distintos a los de una sola.";
                    break;

                case 2:
                    GameManager.Instance?.AddDecision("separados_c1");
                    foreach (var n in alive)
                    {
                        if (GameManager.Instance?.Characters != null &&
                            GameManager.Instance.Characters.TryGetValue(n, out var ch))
                            ch.AddIsolation(15);
                    }

                    narrativeResult =
                        "Cada uno toma una dirección distinta. La mansión los absorbe en silencio. " +
                        "La distancia entre ellos crece con cada paso. " +
                        "Desde algún lugar de la casa, alguien observa cómo se separan.";
                    break;

                default:
                    GameManager.Instance?.AddDecision("hablar_c1");
                    foreach (var n in alive)
                    {
                        if (GameManager.Instance?.Characters != null &&
                            GameManager.Instance.Characters.TryGetValue(n, out var ch))
                            ch.Connect(5);
                    }

                    narrativeResult =
                        "Las respuestas son vagas, calculadas. Pero en los silencios entre " +
                        "las palabras hay más información que en las palabras mismas. " +
                        "Algo se mueve debajo de la superficie de cada frase.";
                    break;
            }

            decisionMade = true;

            HUDDialogPanel.Instance?.ShowObjectDescription("EL GRUPO", narrativeResult);
            HUDManager.Instance?.RefreshIsolationBars();

            if (btnShowDecision != null)
                btnShowDecision.SetActive(false);
        }

        /// <summary>Llamado desde <see cref="DoorInteractable"/> antes de cargar la escena destino.</summary>
        /// <returns><c>true</c> si se puede continuar con la carga; <c>false</c> si la salida quedó bloqueada.</returns>
        public bool OnExitingRoom()
        {
            if (!decisionMade)
            {
                HUDDialogPanel.Instance?.ShowObjectDescription(
                    "ESPERA",
                    "Antes de explorar el resto de la mansión, el grupo debe tomar una decisión.");
                return false;
            }

            GameManager.Instance?.CloseCurrentRoom();
            return true;
        }

        private void OnCharacterDied(string name)
        {
            var fotoObj = GameObject.Find("Obj_FotoGrupo");
            if (fotoObj != null)
                fotoObj.SetActive(true);
        }
    }
}
