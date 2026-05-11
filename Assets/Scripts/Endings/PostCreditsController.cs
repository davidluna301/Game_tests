using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Simonshouse.UI;

namespace Simonshouse.Endings
{
    /// <summary>Secuencia post-créditos (solo tras Final A). <see cref="EnteredFromFinalA"/> debe activarse antes de cargar esta escena.</summary>
    public class PostCreditsController : MonoBehaviour
    {
        /// <summary>La escena <c>PostCredits</c> solo debe ejecutarse si esto es true (lo pone <see cref="EndingsController"/> al salir del Final A).</summary>
        public static bool EnteredFromFinalA { get; set; }

        [Header("Elementos de UI")]
        [SerializeField] private TextMeshProUGUI textTypewriter;
        [SerializeField] private GameObject panelEresTu;
        [SerializeField] private GameObject panelMiraDetras;
        [SerializeField] private TextMeshProUGUI textFinal;
        [SerializeField] private Image glitchOverlay;

        [Header("Timing")]
        [SerializeField] private float typeSpeed = 0.06f;
        [SerializeField] private float pauseShort = 1.5f;
        [SerializeField] private float pauseLong = 3f;

        private void Start()
        {
            if (!EnteredFromFinalA)
            {
                Debug.LogWarning("[PostCredits] Acceso sin Final A — cargando MainMenu.");
                SceneManager.LoadScene("MainMenu");
                return;
            }

            EnteredFromFinalA = false;

            if (panelEresTu != null) panelEresTu.SetActive(false);
            if (panelMiraDetras != null) panelMiraDetras.SetActive(false);
            if (textFinal != null) textFinal.gameObject.SetActive(false);
            if (glitchOverlay != null) glitchOverlay.gameObject.SetActive(false);

            StartCoroutine(PlaySequence());
        }

        private IEnumerator PlaySequence()
        {
            yield return new WaitForSecondsRealtime(2f);

            yield return TypeLine("¿Sigues ahí?");
            yield return new WaitForSecondsRealtime(pauseLong);
            yield return Glitch(1);
            yield return TypeLine("Bien.");
            yield return new WaitForSecondsRealtime(pauseLong);

            yield return TypeLine("Quiero contarte algo.\nAlgo que los personajes no pudieron decirte.");
            yield return new WaitForSecondsRealtime(pauseShort);
            yield return TypeLine("Porque estaban muertos cuando lo entendieron.");
            yield return new WaitForSecondsRealtime(pauseLong);

            yield return TypeLine("Cada decisión que tomaste esta noche fue observada.\nCada persona que murió, murió porque tú elegiste no hablarle.");
            yield return new WaitForSecondsRealtime(pauseLong);

            yield return TypeLine("¿Recuerdas la sexta fotografía?\nLa del tablero de corcho en el estudio.\nLa que tenía el rostro cubierto con cinta negra.");
            yield return new WaitForSecondsRealtime(pauseLong);
            yield return TypeLine("¿Quieres saber quién es?");
            yield return new WaitForSecondsRealtime(pauseLong);

            yield return Glitch(3);

            if (textTypewriter != null) textTypewriter.text = "";
            if (panelEresTu != null) panelEresTu.SetActive(true);
            yield return new WaitForSecondsRealtime(5f);
            if (panelEresTu != null) panelEresTu.SetActive(false);

            yield return TypeLine("Simón te pintó antes de que llegaras.\nEl que observa. El que explora. El que decide quién sobrevive.");
            yield return new WaitForSecondsRealtime(pauseLong);
            yield return TypeLine("Tú eras la sexta persona en la mansión.\nAbriendo puertas. Leyendo cartas. Eligiendo a quién ignorar.");
            yield return new WaitForSecondsRealtime(pauseLong);
            yield return TypeLine("¿Estás seguro de que saliste?");
            yield return new WaitForSecondsRealtime(pauseLong);

            yield return Glitch(5);

            if (textTypewriter != null) textTypewriter.text = "";
            if (panelMiraDetras != null) panelMiraDetras.SetActive(true);
            yield return new WaitForSecondsRealtime(6f);
            if (panelMiraDetras != null) panelMiraDetras.SetActive(false);

            yield return Glitch(8);
            yield return new WaitForSecondsRealtime(2f);

            if (textFinal != null)
            {
                textFinal.gameObject.SetActive(true);
                textFinal.text = "La mansión sigue abierta.\nLa puerta nunca se cerró.\n\nY  T Ú  S I G U E S  A D E N T R O .";
            }

            yield return new WaitForSecondsRealtime(7f);
            SceneManager.LoadScene("MainMenu");
        }

        private IEnumerator TypeLine(string text)
        {
            if (textTypewriter == null) yield break;

            textTypewriter.text = "";
            foreach (char c in text)
            {
                textTypewriter.text += c;
                float wait = (c == '.' || c == '?') ? typeSpeed * 8f
                    : (c == '\n') ? typeSpeed * 3f
                    : typeSpeed;
                yield return new WaitForSecondsRealtime(wait);
            }
        }

        private IEnumerator Glitch(int intensity)
        {
            if (glitchOverlay == null) yield break;

            AudioManager.Instance?.PlayEffect("glitch");
            glitchOverlay.gameObject.SetActive(true);
            for (int i = 0; i < intensity; i++)
            {
                glitchOverlay.color = new Color(Random.value, Random.value, Random.value, 0.5f);
                yield return new WaitForSecondsRealtime(0.06f);
                glitchOverlay.color = Color.clear;
                yield return new WaitForSecondsRealtime(0.03f);
            }

            glitchOverlay.gameObject.SetActive(false);
        }
    }
}
