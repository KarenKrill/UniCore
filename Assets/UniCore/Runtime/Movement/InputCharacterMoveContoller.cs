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

        protected void Awake()
        {
            _characterMoveBehaviour.SpeedModifier = 0.5f;
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
            _characterMoveBehaviour.MoveDirection = new Vector3(_playerActionsProvider.LastMoveDelta.x, 0, _playerActionsProvider.LastMoveDelta.y);
            _characterMoveBehaviour.LookDirection = _playerActionsProvider.LastLookDelta;
        }

        [SerializeField]
        private CharacterMoveBehaviour _characterMoveBehaviour;

        private IBasicPlayerActionsProvider _playerActionsProvider;

        private void OnRun()
        {
            _characterMoveBehaviour.SpeedModifier = 1f;
        }
        private void OnRunCancel()
        {
            _characterMoveBehaviour.SpeedModifier = 0.5f;
        }
        private void OnJump()
        {
            _characterMoveBehaviour.Jump();
        }
        private void OnJumpCancel()
        {
            _characterMoveBehaviour.JumpCancel();
        }
    }
}