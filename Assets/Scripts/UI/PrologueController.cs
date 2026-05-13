using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Simonshouse.UI
{
    public class PrologueController : MonoBehaviour
    {
        private static readonly string[] CharacterOrder = { "Ben", "Lisa", "Robert", "Ana", "Lucas" };

        [Header("Paneles")]
        [SerializeField] private GameObject panelNarration;
        [SerializeField] private GameObject panelCharacters;
        [SerializeField] private GameObject panelMechanics;

        [Header("Textos")]
        [SerializeField] private TextMeshProUGUI textNarration;
        [SerializeField] private float typewriterSpeed = 0.03f;

        [Header("Textos de prólogo")]
        [SerializeField, TextArea(4, 12)]
        private string textStep0 = "Son las tres de la tarde.\n\n"
            + "Una mansión antigua, de paredes que huelen a barniz viejo y a secretos acumulados durante décadas, recibe a cinco desconocidos que, sin embargo, no lo son del todo entre sí.\n\n"
            + "Los une un nombre: Simón. Un pintor. Un hombre que, según les comunicaron hace pocos días, ha muerto.";

        [SerializeField, TextArea(4, 12)]
        private string textStep1 = "Pero cada uno llegó con algo más que condolencias. Cada uno llegó con una razón propia, silenciosa, que no piensa compartir con nadie.\n\n"
            + "Y todos, sin excepción, buscan algo dentro de esa mansión.";

        [SerializeField, TextArea(4, 12)]
        private string textStep2 = "Lo que ninguno sabe es que no están solos.\n\n"
            + "En algún rincón de la casa, alguien más aguarda. Alguien cuya razón para estar ahí es mucho más oscura que todas las demás juntas.\n\n"
            + "El jugador observa. El jugador explora.\nEl jugador decide quién sobrevive.";

        [Header("Personajes (5 líneas: nombre + rol)")]
        [SerializeField] private TextMeshProUGUI[] characterLineTexts = new TextMeshProUGUI[5];

        [Header("Mecánicas")]
        [SerializeField] private TextMeshProUGUI textMechanics;

        [SerializeField, TextArea(8, 20)]
        private string mechanicsNarrative = "EL AISLAMIENTO\n\n"
            + "Cada personaje acumula <b>aislamiento</b> (0–100) según tus decisiones y con quién interactúas en cada sala.\n\n"
            + "• Si al terminar un capítulo alguien no ha sido atendido en esa sala, su aislamiento aumenta.\n"
            + "• Si todos siguen con vida al cierre del capítulo, el grupo recibe un pequeño alivio colectivo.\n"
            + "• Tras cada capítulo, quien lleve el mayor aislamiento en zona crítica puede morir.\n\n"
            + "Explora la mansión, habla con quien elijas y acepta las consecuencias: tú decides quién se acerca a la verdad y quién se queda solo.";

        [Header("Continuar")]
        [SerializeField] private Button btnContinueNarration;
        [SerializeField] private Button btnContinueCharacters;
        [SerializeField] private Button btnContinueMechanics;

        private int step;
        private bool isTyping;
        private Coroutine typingCoroutine;

        private void Awake()
        {
            if (btnContinueNarration != null) btnContinueNarration.onClick.AddListener(NextStep);
            if (btnContinueCharacters != null) btnContinueCharacters.onClick.AddListener(NextStep);
            if (btnContinueMechanics != null) btnContinueMechanics.onClick.AddListener(NextStep);
        }

        private void Start()
        {
            panelCharacters.SetActive(false);
            panelMechanics.SetActive(false);
            panelNarration.SetActive(true);
            ShowStep();
        }

        /// <summary>Llamado por cualquier Btn_Continue del prólogo.</summary>
        public void NextStep()
        {
            if (isTyping)
            {
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);
                textNarration.text = GetStepText(step);
                isTyping = false;
                return;
            }

            step++;
            ShowStep();
        }

        private void ShowStep()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            switch (step)
            {
                case 0:
                case 1:
                case 2:
                    panelNarration.SetActive(true);
                    panelCharacters.SetActive(false);
                    panelMechanics.SetActive(false);
                    typingCoroutine = StartCoroutine(TypewriterEffect(GetStepText(step)));
                    break;
                case 3:
                    panelNarration.SetActive(false);
                    panelCharacters.SetActive(true);
                    panelMechanics.SetActive(false);
                    PopulateCharacterLines();
                    break;
                case 4:
                    panelNarration.SetActive(false);
                    panelCharacters.SetActive(false);
                    panelMechanics.SetActive(true);
                    if (textMechanics != null)
                        textMechanics.text = mechanicsNarrative;
                    break;
                default:
                    SceneManager.LoadScene("Chapter1");
                    break;
            }
        }

        private void PopulateCharacterLines()
        {
            if (characterLineTexts == null || GameManager.Instance?.Characters == null)
                return;

            for (var i = 0; i < CharacterOrder.Length && i < characterLineTexts.Length; i++)
            {
                var line = characterLineTexts[i];
                if (line == null)
                    continue;
                if (!GameManager.Instance.Characters.TryGetValue(CharacterOrder[i], out var c))
                    continue;
                line.text = $"<b>{c.Name}</b>\n<size=95%>{c.Role}</size>";
            }
        }

        private string GetStepText(int s)
        {
            return s switch
            {
                0 => textStep0,
                1 => textStep1,
                2 => textStep2,
                _ => ""
            };
        }

        private IEnumerator TypewriterEffect(string text)
        {
            isTyping = true;
            textNarration.text = "";
            foreach (var c in text)
            {
                textNarration.text += c;
                var wait = c is '.' or '?' or '!' or '\n' ? typewriterSpeed * 4f : typewriterSpeed;
                yield return new WaitForSeconds(wait);
            }

            isTyping = false;
            typingCoroutine = null;
        }
    }
}
