using UnityEngine;

namespace Simonshouse.UI
{
    public class HUDInventoryToggle : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryRoot;

        public void SetInventoryRoot(GameObject root)
        {
            inventoryRoot = root;
            if (inventoryRoot != null)
            {
                inventoryRoot.SetActive(false);
            }
        }

        private void Awake()
        {
            if (inventoryRoot != null)
            {
                inventoryRoot.SetActive(false);
            }
        }

        public void ToggleInventory()
        {
            if (inventoryRoot == null)
            {
                return;
            }

            inventoryRoot.SetActive(!inventoryRoot.activeSelf);
        }
    }
}
