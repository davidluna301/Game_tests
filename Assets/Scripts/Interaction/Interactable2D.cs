using UnityEngine;

namespace Simonshouse.Interaction
{
    /// <summary>Base para elementos clicables 2D. Requiere <see cref="Collider2D"/>.</summary>
    [RequireComponent(typeof(Collider2D))]
    public abstract class Interactable2D : MonoBehaviour
    {
        [Header("Interactable2D — Base")]
        [SerializeField] protected string interactableId;
        [SerializeField] protected bool alreadyUsed;
        [SerializeField] protected bool singleUse = true;

        private InteractableHighlight _highlight;

        public void TryInteract()
        {
            if (singleUse && alreadyUsed)
            {
                OnAlreadyUsed();
                return;
            }

            OnInteract();
            if (singleUse)
                alreadyUsed = true;
        }

        protected abstract void OnInteract();

        protected virtual void OnAlreadyUsed() { }

        public virtual void OnHoverEnter()
        {
            if (_highlight == null)
                _highlight = GetComponent<InteractableHighlight>();
            _highlight?.SetHover(true);
        }

        public virtual void OnHoverExit()
        {
            if (_highlight == null)
                _highlight = GetComponent<InteractableHighlight>();
            _highlight?.SetHover(false);
        }
    }
}
