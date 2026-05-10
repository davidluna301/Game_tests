using System.Collections.Generic;

namespace Simonshouse.UI
{
    [System.Serializable]
    public class DialogChain
    {
        /// ID único, ej. "C1_Robert", "C3_Lucas"
        public string chainId;

        /// Nombre del personaje que habla (para mostrar sprite en el HUD).
        public string characterName;

        /// Pista global necesaria para activar esta cadena. Vacío = siempre.
        public string requiredClue;

        /// Ítem de inventario necesario para esta cadena. Vacío = siempre.
        public string requiredItem;

        public List<DialogLine> lines = new List<DialogLine>();
    }
}
