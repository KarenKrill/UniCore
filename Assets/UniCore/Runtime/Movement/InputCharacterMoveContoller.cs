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
            _characterMoveBehaviour.GravityModifier = 0.5f;
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
            if (_characterMoveBehaviour.IsPulsedUp)
            {
                if (!_isJumpPressed && _characterMoveBehaviour.IsFalling) // если короткое нажатие
                {
                    _characterMoveBehaviour.GravityModifier = 1f; // ускоряем прыжок
                }
                else
                {
                    _characterMoveBehaviour.GravityModifier = 0.5f;
                }
            }
        }

        [SerializeField]
        private CharacterMoveBehaviour _characterMoveBehaviour;
        [SerializeField]
        private float _jumpHeight = 2.0f, _jumpHorizontalSpeed = 3.0f;
        [SerializeField]
        private float _jumpButtonGracePeriod = 0.2f;

        private IBasicPlayerActionsProvider _playerActionsProvider;
        private bool _isJumpPressed = false;

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
            _characterMoveBehaviour.PulseUp(_jumpHeight, _jumpButtonGracePeriod, _jumpHorizontalSpeed);
            _isJumpPressed = true;
        }
        private void OnJumpCancel()
        {
            _isJumpPressed = false;
        }
    }
}