using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Simonshouse.UI
{
    /// <summary>Entrada Lobby, foto de grupo tras primera muerte, panel de decisión C1.</summary>
    public class LobbyController : MonoBehaviour
    {
        [Header("Panel de decisión")]
        [SerializeField] private GameObject panelDecision;
        [SerializeField] private Button btnDecision1;
        [SerializeField] private Button btnDecision2;
        [SerializeField] private Button btnDecision3;
        [SerializeField] private Button btnConvocarGrupo;

        private void Awake()
        {
            if (btnConvocarGrupo != null)
                btnConvocarGrupo.onClick.AddListener(ShowDecisionPanel);
            if (btnDecision1 != null)
                btnDecision1.onClick.AddListener(() => ApplyLobbyDecision(1));
            if (btnDecision2 != null)
                btnDecision2.onClick.AddListener(() => ApplyLobbyDecision(2));
            if (btnDecision3 != null)
                btnDecision3.onClick.AddListener(() => ApplyLobbyDecision(3));
        }

        private void Start()
        {
            GameManager.Instance?.EnterRoom("Lobby");
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnCharacterDied += ActivateGroupPhoto;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnCharacterDied -= ActivateGroupPhoto;
        }

        /// <summary>Desde el botón «Convocar al grupo» u otro UI en escena.</summary>
        public void ShowDecisionPanel()
        {
            if (panelDecision != null)
                panelDecision.SetActive(true);
        }

        private void HideDecisionPanel()
        {
            if (panelDecision != null)
                panelDecision.SetActive(false);
        }

        private static void ActivateGroupPhoto(string _)
        {
            var fotoGrupo = GameObject.Find("Obj_FotoGrupo");
            if (fotoGrupo != null)
                fotoGrupo.SetActive(true);
        }

        /// <summary>Misma lógica que las tres opciones de decisión del capítulo 1 (texto y stats), sin cerrar capítulo.</summary>
        private void ApplyLobbyDecision(int option)
        {
            HideDecisionPanel();

            if (GameManager.Instance?.Characters == null)
                return;

            var alive = GameManager.Instance.GetAliveCharacters();

            switch (option)
            {
                case 1:
                    GameManager.Instance.AddDecision("grupo_unido_c1");
                    foreach (var n in alive)
                        GameManager.Instance.Characters[n].Connect(10);
                    RunLobbyNarration(
                        "El grupo acepta, aunque con reservas. Hay algo reconfortante en moverse juntos por una casa que ninguno conoce del todo. Los pasos de cinco personas suenan distintos a los de una sola.");
                    break;

                case 2:
                    GameManager.Instance.AddDecision("separados_c1");
                    foreach (var n in alive)
                        GameManager.Instance.Characters[n].AddIsolation(15);
                    RunLobbyNarration(
                        "Cada uno toma una dirección distinta. La mansión los absorbe en silencio. La distancia entre ellos crece con cada paso. Desde algún lugar de la casa, alguien observa cómo se separan.");
                    break;

                case 3:
                    GameManager.Instance.AddDecision("hablar_c1");
                    foreach (var n in alive)
                        GameManager.Instance.Characters[n].Connect(5);
                    RunLobbyNarration(
                        "Las respuestas son vagas, calculadas. Pero en los silencios entre las palabras hay más información que en las palabras mismas. Algo se mueve debajo de la superficie de cada frase.");
                    break;
            }
        }

        private static void RunLobbyNarration(string narration)
        {
            var chain = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new() { isNarration = true, content = narration }
                }
            };
            DialogManager.Instance?.StartChain(chain);
        }
    }
}
