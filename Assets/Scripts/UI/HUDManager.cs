using UnityEngine;

namespace Simonshouse.UI
{
    public class HUDManager : MonoBehaviour
    {
        public static HUDManager Instance { get; private set; }

        [Header("Isolation HUD")]
        [SerializeField] private GameObject panelIsolationHUD;
        [SerializeField] private GameObject isolationBarPrefab;
        [SerializeField] private Transform isolationBarsContainer;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void ShowIsolationHUD()
        {
            if (panelIsolationHUD != null) panelIsolationHUD.SetActive(true);
            RefreshIsolationBars();
        }

        public void HideIsolationHUD()
        {
            if (panelIsolationHUD != null) panelIsolationHUD.SetActive(false);
        }

        public void RefreshIsolationBars()
        {
            if (isolationBarsContainer == null || isolationBarPrefab == null) return;
            if (GameManager.Instance == null || GameManager.Instance.Characters == null) return;

            foreach (Transform child in isolationBarsContainer)
                Destroy(child.gameObject);

            foreach (var kv in GameManager.Instance.Characters)
            {
                var bar = Instantiate(isolationBarPrefab, isolationBarsContainer);
                var barUI = bar.GetComponent<IsolationBarUI>();
                if (barUI != null) barUI.Setup(kv.Value);
            }
        }
    }
}

