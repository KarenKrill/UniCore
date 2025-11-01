using UnityEngine;

using KarenKrill.UniCore.Input.Abstractions;
using KarenKrill.UniCore.Interactions.Abstractions;

namespace KarenKrill.UniCore.Interactions
{
    public class RaycastInteractionDetector : RaycastInteractionDetectorBase
    {
        public void Initialize(IBasicPlayerActionsProvider playerActionsProvider, IInteractionTargetRegistry interactionTargetRegistry)
        {
            _playerActionsProvider = playerActionsProvider;
            base.Initialize(interactionTargetRegistry);
        }

        protected override void InputSubscribe()
        {
            _playerActionsProvider.Look += OnLookOrMove;
            _playerActionsProvider.Move += OnLookOrMove;
            _playerActionsProvider.Interact += OnInteract;
        }
        protected override void InputUnsubscribe()
        {
            _playerActionsProvider.Look -= OnLookOrMove;
            _playerActionsProvider.Move -= OnLookOrMove;
            _playerActionsProvider.Interact -= OnInteract;
        }

        private IBasicPlayerActionsProvider _playerActionsProvider;

        private void OnLookOrMove(Vector2 delta)
        {
            var ray = new Ray(_interactorEyePoint.position, _interactorLookPoint.position - _interactorEyePoint.position);
            OnLookChanged(_interactor, ray);
        }
        private void OnInteract() => OnInteract(_interactor);

        [SerializeField]
        private InteractorBase _interactor;
        [SerializeField]
        private Transform _interactorEyePoint;
        [SerializeField]
        private Transform _interactorLookPoint;
    }
}
