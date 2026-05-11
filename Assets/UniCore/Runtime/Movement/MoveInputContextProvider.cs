using UnityEngine;

using KarenKrill.UniCore.Input.Abstractions;

namespace KarenKrill.UniCore.Movement
{
    public class MoveInputContextProvider : MonoBehaviour
    {
        public MoveInputContext Context => _moveContext;

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
            _moveContext.MoveDelta = _playerActionsProvider.LastMoveDelta;
            _moveContext.LookDelta = _playerActionsProvider.LastLookDelta;
        }

        private readonly MoveInputContext _moveContext = new();

        private IBasicPlayerActionsProvider _playerActionsProvider;

        private void OnRun()
        {
            _moveContext.IsSprintPressed = true;
        }
        private void OnRunCancel()
        {
            _moveContext.IsSprintPressed = false;
        }
        private void OnJump()
        {
            _moveContext.IsJumpPressed = true;
            _moveContext.LastJumpStartTime = Time.time;
        }
        private void OnJumpCancel()
        {
            _moveContext.IsJumpPressed = false;
        }
    }
}