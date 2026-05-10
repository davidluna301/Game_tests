using UnityEngine;

namespace Simonshouse.UI
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Simonshouse/ItemData")]
    public class ItemData : ScriptableObject
    {
        public string itemId;
        public string itemName;
        [TextArea(2, 5)]
        public string description;
        public Sprite sprite;          // asignar en postproducción
        public string ownerCharacter;  // "Ben", "Lisa", etc.
    }
}
