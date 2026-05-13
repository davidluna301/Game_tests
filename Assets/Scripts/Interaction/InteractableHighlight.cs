using UnityEngine;

namespace Simonshouse.Interaction
{
    /// <summary>Feedback visual de hover (placeholder hasta postproducción).</summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class InteractableHighlight : MonoBehaviour
    {
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(1f, 0.9f, 0.5f);

        private SpriteRenderer _sr;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        public void SetHover(bool isHover)
        {
            if (_sr != null)
                _sr.color = isHover ? hoverColor : normalColor;
        }
    }
}
