namespace Simonshouse.UI
{
    [System.Serializable]
    public class DialogLine
    {
        public string speakerName;

        [UnityEngine.TextArea(2, 6)]
        public string content;

        /// Si true, se muestra sin nombre (acotación de narrador).
        public bool isNarration;

        /// Pista requerida para que esta línea aparezca. Vacío = siempre visible.
        public string requiredClue;

        /// Ítem de inventario requerido para esta línea. Vacío = siempre visible.
        public string requiredItem;
    }
}
