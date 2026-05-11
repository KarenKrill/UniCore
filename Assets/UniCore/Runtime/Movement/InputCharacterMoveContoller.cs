using UnityEngine;

using KarenKrill.UniCore.Input.Abstractions;

namespace KarenKrill.UniCore.Movement
{
    public class InputCharacterMoveContoller : MonoBehaviour
    {
        public void Initialize(IBasicPlayerActionsProvider playerActionsProvider)
        {
            _playerActionsProvider = playerActionsProvider;
        }

        protected void OnEnable()
        {
            _playerActionsProvider.Sprint += OnRun;
            _playerActionsProvider.SprintCancel += OnRunCancel;
            _playerActionsProvider.Jump += OnJump;
            _playerActionsProvider.JumpCancel += OnJumpCancel;
        }
        protected void OnDisable()
        {
            _playerActionsProvider.Sprint -= OnRun;
            _playerActionsProvider.SprintCancel -= OnRunCancel;
            _playerActionsProvider.Jump -= OnJump;
            _playerActionsProvider.JumpCancel -= OnJumpCancel;
        }
        protected void Update()
        {
            _MoveContext.MoveDelta = _playerActionsProvider.LastMoveDelta;
            _MoveContext.LookDelta = _playerActionsProvider.LastLookDelta;
        }

        private MoveInputContext _MoveContext => _characterMoveBehaviour.MoveInputContext;

        [SerializeField]
        private CharacterMoveBehaviour _characterMoveBehaviour;

        private IBasicPlayerActionsProvider _playerActionsProvider;

        private void OnRun()
        {
            _MoveContext.IsSprintPressed = true;
        }
        private void OnRunCancel()
        {
            _MoveContext.IsSprintPressed = false;
        }
        private void OnJump()
        {
            _MoveContext.IsJumpPressed = true;
            _MoveContext.LastJumpStartTime = Time.time;
        }
        private void OnJumpCancel()
        {
            _MoveContext.IsJumpPressed = false;
        }
    }
}