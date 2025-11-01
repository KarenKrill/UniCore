using UnityEngine;

namespace KarenKrill.UniCore.Interactions
{
    using Abstractions;

    public abstract class InteractionTargetBase : MonoBehaviour, IInteractionTarget
    {
        public abstract IInteractable Interactable { get; }
        
        public virtual void Initialize(IInteractionTargetRegistry interactionTargetRegistry)
        {
            _interactionTargetRegistry = interactionTargetRegistry;
        }

        protected virtual void OnEnable()
        {
            _interactionTargetRegistry.Register(this);
        }
        protected virtual void OnDisable()
        {
            _interactionTargetRegistry.Unregister(this);
        }

        private IInteractionTargetRegistry _interactionTargetRegistry;
    }
}
