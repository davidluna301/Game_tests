using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Simonshouse.UI
{
    public class IsolationBarUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textName;
        [SerializeField] private Slider sliderIsolation;
        [SerializeField] private TextMeshProUGUI textLabel;
        [SerializeField] private Image fillImage;

        [Header("Colores por nivel de aislamiento")]
        [SerializeField] private Color colorStable = Color.green;
        [SerializeField] private Color colorMedium = Color.yellow;
        [SerializeField] private Color colorHigh = new(1f, 0.5f, 0f);
        [SerializeField] private Color colorCritical = Color.red;
        [SerializeField] private Color colorDead = Color.gray;

        public void Setup(CharacterData c)
        {
            if (c == null) return;

            if (textName != null) textName.text = c.IsAlive ? c.Name : $"{c.Name}  ✝";
            if (sliderIsolation != null) sliderIsolation.value = c.Isolation / 100f;

            if (!c.IsAlive)
            {
                if (fillImage != null) fillImage.color = colorDead;
                if (textLabel != null) textLabel.text = "✝ MUERTO";
                return;
            }

            switch (c.GetIsolationLevel())
            {
                case IsolationLevel.Stable:
                    if (fillImage != null) fillImage.color = colorStable;
                    if (textLabel != null) textLabel.text = "✓ ESTABLE";
                    break;
                case IsolationLevel.Medium:
                    if (fillImage != null) fillImage.color = colorMedium;
                    if (textLabel != null) textLabel.text = "~ MEDIO";
                    break;
                case IsolationLevel.High:
                    if (fillImage != null) fillImage.color = colorHigh;
                    if (textLabel != null) textLabel.text = "▲ ALTO";
                    break;
                case IsolationLevel.Critical:
                    if (fillImage != null) fillImage.color = colorCritical;
                    if (textLabel != null) textLabel.text = "⚠ CRÍTICO";
                    break;
            }
        }
    }
}

