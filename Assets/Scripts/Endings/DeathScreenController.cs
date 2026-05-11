using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Simonshouse.UI;

namespace Simonshouse.Endings
{
    public class DeathScreenController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI textCharacterName;
        [SerializeField] private Image imgCharacterSprite;
        [SerializeField] private TextMeshProUGUI textPart1;
        [SerializeField] private TextMeshProUGUI textPart2;
        [SerializeField] private TextMeshProUGUI textPart3;
        [SerializeField] private TextMeshProUGUI textPart4;
        [SerializeField] private TextMeshProUGUI textTensionEvent;
        [SerializeField] private GameObject btnContinue;

        [Header("Timing")]
        [SerializeField] private float pauseShort = 1.5f;
        [SerializeField] private float pauseLong = 2.5f;

        [Header("Character Sprites (postproducción)")]
        [SerializeField] private Sprite spriteBen;
        [SerializeField] private Sprite spriteLisa;
        [SerializeField] private Sprite spriteRobert;
        [SerializeField] private Sprite spriteAna;
        [SerializeField] private Sprite spriteLucas;

        private void Start()
        {
            if (btnContinue != null) btnContinue.SetActive(false);
            ClearTexts();

            string who = ChapterFlowManager.Instance != null
                ? ChapterFlowManager.Instance.GetPendingDeathCharacter()
                : null;
            if (string.IsNullOrEmpty(who))
            {
                OnContinueClicked();
                return;
            }

            StartCoroutine(PlayCinematic(who));
        }

        private void ClearTexts()
        {
            if (textPart1 != null) textPart1.text = "";
            if (textPart2 != null) textPart2.text = "";
            if (textPart3 != null) textPart3.text = "";
            if (textPart4 != null) textPart4.text = "";
            if (textTensionEvent != null) textTensionEvent.text = "";
        }

        private IEnumerator PlayCinematic(string charName)
        {
            if (textCharacterName != null)
                textCharacterName.text = charName.ToUpper();

            AssignSprite(charName);
            yield return new WaitForSecondsRealtime(1f);

            var (p1, p2, p3, p4) = GetDeathTexts(charName);

            if (textPart1 != null) textPart1.text = p1;
            yield return new WaitForSecondsRealtime(pauseLong);

            if (textPart2 != null) textPart2.text = p2;
            yield return new WaitForSecondsRealtime(pauseShort);

            if (textPart3 != null) textPart3.text = p3;
            yield return new WaitForSecondsRealtime(pauseLong);

            if (textPart4 != null) textPart4.text = p4;
            yield return new WaitForSecondsRealtime(pauseShort);

            if (textTensionEvent != null) textTensionEvent.text = GetTensionEvent();
            yield return new WaitForSecondsRealtime(pauseLong);

            if (btnContinue != null) btnContinue.SetActive(true);
        }

        private void AssignSprite(string name)
        {
            if (imgCharacterSprite == null) return;
            Sprite s = name switch
            {
                "Ben" => spriteBen,
                "Lisa" => spriteLisa,
                "Robert" => spriteRobert,
                "Ana" => spriteAna,
                "Lucas" => spriteLucas,
                _ => null
            };
            if (s != null) imgCharacterSprite.sprite = s;
        }

        private (string, string, string, string) GetDeathTexts(string name) => name switch
        {
            "Ben" => (
                "Ben llevaba rato callado. En algún momento se levantó del sillón y caminó hacia el pasillo del ala este sin decir nada. Nadie lo siguió.",
                "Veinte minutos después, alguien fue a buscarlo.",
                "Ben estaba en el suelo, boca arriba. Su corbata enrollada alrededor de su cuello con una precisión quirúrgica. Tres vueltas.",
                "El libro de cuentas sobre su pecho. Escrito en rojo: «SALDADO.»"
            ),
            "Lisa" => (
                "Lisa dijo que necesitaba verificar algo en la galería. «Vuelvo en cinco minutos.» Nadie la acompañó.",
                "No volvió en cinco minutos. Ni en diez. Ni en veinte.",
                "La encontraron en el taller, sentada contra la pared. La mancha crecía desde debajo de ella. Última línea en su libreta: «Hay alguien detrás de m—»",
                "Sobre su cabeza, escrito con los dedos: «SILENCIO.»"
            ),
            "Robert" => (
                "Robert se fue quedando cada vez más quieto. En algún momento subió al segundo piso solo.",
                "Nadie lo escuchó gritar. Eso fue lo peor.",
                "Lo encontraron en la habitación de Simón. En cada palma: un objeto. La fotografía. La carta. Manchada de sangre en las esquinas.",
                "No había heridas visibles. Su expresión era la de alguien que vio algo peor que morir."
            ),
            "Ana" => (
                "Ana empezó a temblar. «No puedo seguir aquí.» Se alejó hacia la escalera. Escucharon una puerta cerrarse.",
                "Después, un golpe. Húmedo. Final.",
                "La encontraron al pie de la escalera del ala norte. La barandilla del segundo piso estaba rota desde atrás. Empujada.",
                "El estuche de joyas estaba en el suelo. Abierto. Vacío. Dentro: un espejo pequeño."
            ),
            "Lucas" => (
                "Lucas dejó de hablar primero. La última vez que alguien lo vio estaba frente al cuadro del niño con el relicario. Cuando se giraron, ya no estaba.",
                "Su maletín seguía en el lobby. Con un rastro de gotas oscuras hacia la puerta del sótano.",
                "Lo encontraron al pie de la escalera del sótano. Tenía marcas en las muñecas. Dos juegos de huellas en el polvo.",
                "El relicario en su mano cerrada. «Para Lucas. Siempre.» Murió agarrado a un nombre que ni siquiera era el suyo."
            ),
            _ => (
                "El personaje se separó del grupo.",
                "Nadie lo acompañó.",
                "El cuerpo estaba frío cuando lo encontraron.",
                "El aislamiento lo convirtió en presa fácil."
            )
        };

        private string GetTensionEvent()
        {
            if (GameManager.Instance == null || GameManager.Instance.Characters == null)
                return "Ya no hay sonidos extraños. El silencio de la mansión es absoluto. El tipo de silencio que solo existe cuando algo terrible ya terminó. O está a punto de pasar de nuevo.";

            int dead = 0;
            foreach (var c in GameManager.Instance.Characters.Values)
                if (!c.IsAlive) dead++;

            return dead switch
            {
                1 => "Las luces del lobby parpadean. Al volver, hay una fotografía nueva en la chimenea. El rostro del muerto lleva una X dibujada con sangre.",
                2 => "Un sonido recorre la mansión. Algo se arrastra por el piso de arriba. Se detiene encima del grupo. Luego, gotas rojas a través del techo.",
                3 => "En la mesa del lobby, una nota con huellas dactilares: «QUEDAN POCOS.» Más abajo: «Los estoy mirando ahora mismo.»",
                _ => "Ya no hay sonidos extraños. El silencio de la mansión es absoluto. El tipo de silencio que solo existe cuando algo terrible ya terminó. O está a punto de pasar de nuevo."
            };
        }

        public void OnContinueClicked()
        {
            Debug.Log("[DeathScreen] OnContinueClicked → ChapterFlowManager.OnDeathScreenFinished()");
            ChapterFlowManager.Instance?.OnDeathScreenFinished();
        }
    }
}
