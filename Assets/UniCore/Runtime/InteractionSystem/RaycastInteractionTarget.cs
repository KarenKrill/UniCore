using UnityEngine;

namespace KarenKrill.UniCore.Interactions
{
    using Abstractions;

    [RequireComponent(typeof(Collider))]
    public class RaycastInteractionTarget : InteractionTargetBase, IInteractionTarget
    {
        public override IInteractable Interactable => _interactable;

        [SerializeField]
        private InteractableBase _interactable;
    }
}
