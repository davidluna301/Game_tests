using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Simonshouse.UI;

namespace Simonshouse.Endings
{
    public class EndingsController : MonoBehaviour
    {
        [Header("Paneles de final")]
        [SerializeField] private GameObject panelEndingA;
        [SerializeField] private GameObject panelEndingB;
        [SerializeField] private GameObject panelEndingC;

        [Header("Textos — Final A")]
        [SerializeField, TextArea(4, 12)]
        private string textA_Act1 =
            "Caminaron hacia la puerta principal. Simón iba entre ellos, apoyado en alguien, arrastrando los pies. Nadie habló.\n\n"
            + "La puerta principal estaba abierta. El aire de la noche entró como algo vivo, frío, que olía a tierra mojada y a libertad.\n\n"
            + "Dieron un paso afuera. Dos. Tres.\n\n"
            + "Entonces la puerta se cerró detrás de ellos. No. No se cerró. Alguien la cerró. Desde adentro.";

        [SerializeField, TextArea(4, 12)] private string textA_Act2 = "";

        [SerializeField, TextArea(4, 12)]
        private string textA_Act3 =
            "La noticia de la muerte de Simón fue falsa. El sobre tenía huellas. Las huellas coincidían con las de Simón.\n\n"
            + "Simón envió la noticia de su propia muerte.\n"
            + "Simón preparó los objetos.\n"
            + "Simón los atrajo a la mansión.\n\n"
            + "Pero Simón no se ató a sí mismo a una silla en el ala norte. Alguien más hizo eso. Alguien que llegó primero.\n\n"
            + "La sexta persona de la fotografía. La del rostro cubierto con cinta negra.";

        [SerializeField, TextArea(4, 12)]
        private string textA_Act4 =
            "Seis meses después, el cartel había desaparecido.\n"
            + "Las puertas estaban abiertas.\n"
            + "Y cinco personas recibieron invitaciones.\n\n"
            + "Cinco sobres. Cinco nombres nuevos. Cinco razones para ir a una mansión antigua.\n\n"
            + "Las invitaciones estaban escritas a mano. Con la letra de Simón. Un hombre que llevaba tres meses muerto.\n\n"
            + "Nuevos nombres. Nuevos secretos.\n"
            + "El mismo final.";

        [SerializeField] private TextMeshProUGUI displayTextA;

        [Header("Textos — Final B")]
        [SerializeField, TextArea(4, 12)]
        private string textB_Main =
            "Cada uno sostiene lo que vino a buscar. La casa devuelve el eco de pasos que ya no son de huéspedes, sino de testigos.\n\n"
            + "Simón asiente sin celebración. Fuera, el amanecer parece barato comparado con lo que habéis pagado para llegar hasta aquí.";

        [SerializeField] private TextMeshProUGUI displayTextB;

        [Header("Textos — Final C")]
        [SerializeField, TextArea(4, 12)]
        private string textC_Main =
            "No todos recuperaron lo suyo. No todos quedaron iguales. La mansión os deja ir, pero el reparto del miedo no ha sido justo.";

        [SerializeField] private TextMeshProUGUI displayTextC;

        [Header("Flujo")]
        [SerializeField] private float secondsPerActA = 4f;
        [Tooltip("Visible al terminar cada final. En el Inspector: OnClick → EndingsController.OnEndingFinished.")]
        [SerializeField] private Button btnContinue;
        [SerializeField] private TextMeshProUGUI btnContinueLabel;

        private string _activeEnding = "";
        private Coroutine _playA;

        private void Start()
        {
            SetPanelsActive(false, false, false);
            SetContinueVisible(false);

            _activeEnding = SafeDetermineEnding();
            Debug.Log($"[Ending] Final determinado: {_activeEnding}");

            switch (_activeEnding)
            {
                case "A":
                    _playA = StartCoroutine(PlayEndingA());
                    break;
                case "B":
                    PlayEndingB();
                    break;
                case "C":
                    PlayEndingC();
                    break;
                default:
                    Debug.LogWarning($"[Ending] Valor inesperado '{_activeEnding}', usando Final C.");
                    _activeEnding = "C";
                    PlayEndingC();
                    break;
            }
        }

        private static string SafeDetermineEnding()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("[Ending] GameManager.Instance es null.");
                return "C";
            }

            return GameManager.Instance.DetermineEnding();
        }

        private IEnumerator PlayEndingA()
        {
            if (panelEndingA != null)
                panelEndingA.SetActive(true);

            string act3 = textA_Act3;
            if (GameManager.Instance != null && GameManager.Instance.HasClue("note_coat"))
            {
                act3 += "\n\n«No confíes en nadie que llegue antes que tú.» Era una advertencia de Simón para sí mismo. "
                    + "Una advertencia que no siguió.";
            }

            var acts = new List<string> { textA_Act1, textA_Act2, act3, textA_Act4 };
            foreach (var act in acts)
            {
                if (string.IsNullOrWhiteSpace(act))
                    continue;

                if (displayTextA != null)
                    displayTextA.text = act;

                yield return new WaitForSeconds(secondsPerActA);
            }

            SetContinueVisible(true);
        }

        private void PlayEndingB()
        {
            if (panelEndingB != null)
                panelEndingB.SetActive(true);
            if (displayTextB != null)
                displayTextB.text = textB_Main;

            SetContinueVisible(true);
        }

        private void PlayEndingC()
        {
            if (panelEndingC != null)
                panelEndingC.SetActive(true);
            var alive = GameManager.Instance != null
                ? GameManager.Instance.GetAliveCharacters()
                : new List<string>();

            string survivors = alive.Count > 0 ? string.Join(", ", alive) : "nadie";
            string text = textC_Main + "\n\nLos que pudieron salir: " + survivors;

            if (displayTextC != null)
                displayTextC.text = text;

            SetContinueVisible(true);
        }

        private void SetPanelsActive(bool a, bool b, bool c)
        {
            if (panelEndingA != null) panelEndingA.SetActive(a);
            if (panelEndingB != null) panelEndingB.SetActive(b);
            if (panelEndingC != null) panelEndingC.SetActive(c);
        }

        private void SetContinueVisible(bool visible)
        {
            if (btnContinue != null)
                btnContinue.gameObject.SetActive(visible);

            if (visible && btnContinueLabel != null && string.IsNullOrEmpty(btnContinueLabel.text))
                btnContinueLabel.text = "Continuar";
        }

        /// <summary>Enlazar desde un botón de UI al terminar de leer el final.</summary>
        public void OnEndingFinished()
        {
            if (_playA != null)
            {
                StopCoroutine(_playA);
                _playA = null;
            }

            string ending = GameManager.Instance != null
                ? GameManager.Instance.DetermineEnding()
                : _activeEnding;

            if (string.IsNullOrEmpty(ending))
                ending = "C";

            SceneManager.LoadScene(ending == "A" ? "PostCredits" : "MainMenu");
        }
    }
}
