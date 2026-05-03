using KarenKrill.UniCore.Input.Abstractions;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour, IBasicPlayerActionsProvider
{
    public Vector2 LastLookDelta { get; private set; }
    public Vector2 LastMoveDelta { get; private set; }
    public bool IsSprintActive { get; private set; }
    public bool IsCrouchActive { get; private set; }
    public bool IsJumpActive { get; private set; }
    public bool IsAttackActive { get; private set; }
    public bool IsInteractActive { get; private set; }

#pragma warning disable CS0067
    public event LookDelegate Look;
    public event Action LookCancel;
    public event Action MoveStarted;
    public event MoveDelegate Move;
    public event Action MoveCancel;
    public event Action Sprint;
    public event Action SprintCancel;
    public event Action Crouch;
    public event Action CrouchCancel;
    public event Action Jump;
    public event Action JumpCancel;
    public event Action Attack;
    public event Action AttackCancel;
    public event Action Interact;
    public event Action InteractCancel;
    public event Action Pause;
#pragma warning restore

    [SerializeField]
    private InputActionReference _moveActionReference;
    [SerializeField]
    private InputActionReference _lookActionReference;
    [SerializeField]
    private InputActionReference _jumpActionReference;
    [SerializeField]
    private InputActionReference _sprintActionReference;

    private void OnApplicationFocus(bool focus)
    {
        Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnEnable()
    {
        _moveActionReference.action.started += OnMoveActionPerformed;
        _moveActionReference.action.performed += OnMoveActionPerformed;
        _moveActionReference.action.canceled += OnMoveActionPerformed;
        _lookActionReference.action.performed += OnLookActionPerformed;
        _lookActionReference.action.canceled += OnLookActionPerformed;
        _jumpActionReference.action.performed += OnJumpActionPerformed;
        _jumpActionReference.action.canceled += OnJumpActionPerformed;
        _sprintActionReference.action.performed += OnSprintActionPerformed;
        _sprintActionReference.action.canceled += OnSprintActionPerformed;
    }

    private void OnDisable()
    {
        _moveActionReference.action.started -= OnMoveActionPerformed;
        _moveActionReference.action.performed -= OnMoveActionPerformed;
        _moveActionReference.action.canceled -= OnMoveActionPerformed;
        _lookActionReference.action.performed -= OnLookActionPerformed;
        _lookActionReference.action.canceled -= OnLookActionPerformed;
        _jumpActionReference.action.performed -= OnJumpActionPerformed;
        _jumpActionReference.action.canceled -= OnJumpActionPerformed;
        _sprintActionReference.action.performed -= OnSprintActionPerformed;
        _sprintActionReference.action.canceled -= OnSprintActionPerformed;
    }

    private void OnMoveActionPerformed(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            LastMoveDelta = Vector2.zero;
            MoveCancel?.Invoke();
        }
        else
        {
            var moveDelta = ctx.action.ReadValue<Vector2>();
            LastMoveDelta = moveDelta;
            if (ctx.started)
            {
                MoveStarted?.Invoke();
            }
            else
            {
                Move?.Invoke(moveDelta);
            }
        }
    }
    private void OnLookActionPerformed(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            LastLookDelta = Vector2.zero;
            LookCancel?.Invoke();
        }
        else if(ctx.performed)
        {
            var delta = ctx.action.ReadValue<Vector2>();
            LastLookDelta = delta;
            Look?.Invoke(delta);
        }
    }
    private void OnJumpActionPerformed(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Jump?.Invoke();
        }
        else if (ctx.canceled)
        {
            JumpCancel?.Invoke();
        }
    }
    private void OnSprintActionPerformed(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Sprint?.Invoke();
        }
        else if (ctx.canceled)
        {
            SprintCancel?.Invoke();
        }
    }

}
