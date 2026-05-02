using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Simonshouse.UI
{
    public class Chapter1NarrativeController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI paragraphText;
        [SerializeField] private float fadeDuration = 1.2f;
        [SerializeField] private float autoAdvanceSeconds = 20f;

        [TextArea(4, 8)]
        [SerializeField]
        private string[] paragraphs =
        {
            "Europa, 1943. Una mansion antigua en las afueras de la ciudad recibe a cinco desconocidos convocados por una misma noticia: la muerte repentina de Simon, un pintor de renombre. Aunque ninguno se conoce bien, todos comparten algo en comun: una relacion con el difunto cargada de deudas, secretos y culpas no confesadas. Ben, Lisa, Robert, Ana y Lucas llegan con condolencias en los labios y motivos ocultos en el bolsillo.",
            "Pero Simon no esta muerto. Dentro de la mansion, pistas dispersas con calculada precision revelan que el pintor fue retenido por la fuerza en el ala norte, que alguien lo atrajo deliberadamente a esa trampa, y que los cinco visitantes fueron convocados con el mismo proposito: cada uno tiene algo que Simon guardo para ellos, colocado exactamente donde lo buscarian. La mansion es un escenario disenado, y sus huespedes, piezas de un juego que no pidieron jugar.",
            "La amenaza no viene solo del asesino. Es el aislamiento lo que mata primero: quien se separa del grupo, quien calla demasiado, quien carga su secreto en soledad, se convierte en presa facil. El jugador observa, explora y decide quien recibe atencion y quien es ignorado, determinando asi quien sobrevive la noche. Y sin saberlo aun, tambien decide su propio destino, porque desde el principio hay una sexta persona en la mansion cuyo rostro nadie ha podido ver."
        };

        private void Awake()
        {
            if (paragraphText == null)
            {
                paragraphText = GetComponentInChildren<TextMeshProUGUI>(true);
            }

            if (paragraphText == null)
            {
                Debug.LogError("Chapter1NarrativeController: No TextMeshProUGUI found.");
                enabled = false;
            }
        }

        private void Start()
        {
            StartCoroutine(RunNarrative());
        }

        private IEnumerator RunNarrative()
        {
            SetTextAlpha(0f);

            for (int i = 0; i < paragraphs.Length; i++)
            {
                paragraphText.text = paragraphs[i];

                yield return FadeText(0f, 1f);

                if (i < paragraphs.Length - 1)
                {
                    yield return WaitForAdvance();
                    yield return FadeText(1f, 0f);
                }
            }
        }

        private IEnumerator WaitForAdvance()
        {
            float elapsed = 0f;

            while (elapsed < autoAdvanceSeconds)
            {
                if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                {
                    yield break;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator FadeText(float from, float to)
        {
            if (fadeDuration <= 0f)
            {
                SetTextAlpha(to);
                yield break;
            }

            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                SetTextAlpha(Mathf.Lerp(from, to, t));
                yield return null;
            }

            SetTextAlpha(to);
        }

        private void SetTextAlpha(float alpha)
        {
            Color color = paragraphText.color;
            color.a = alpha;
            paragraphText.color = color;
        }
    }
}
