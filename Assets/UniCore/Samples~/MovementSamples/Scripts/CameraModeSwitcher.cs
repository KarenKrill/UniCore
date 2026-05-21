using KarenKrill.UniCore.Movement;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraModeSwitcher : MonoBehaviour
{
    [SerializeField]
    private CharacterMoveBehaviour _characterMoveBehaviour;
    [SerializeField]
    private CinemachineCamera _fpsCamera;
    [SerializeField]
    private CinemachineCamera _thirdPersonCamera;

    [SerializeField]
    private InputActionReference _switchActionReference;

    private void OnEnable()
    {
        _switchActionReference.action.performed += OnSwitchActionPerformed;
    }

    private void OnDisable()
    {
        _switchActionReference.action.performed -= OnSwitchActionPerformed;
    }

    private void OnSwitchActionPerformed(InputAction.CallbackContext ctx)
    {
        var cameraType = _characterMoveBehaviour.CameraType;
        if (cameraType == KarenKrill.UniCore.Movement.CameraType.ThirdPerson)
        {
            _thirdPersonCamera.gameObject.SetActive(false);
            _fpsCamera.gameObject.SetActive(true);
            cameraType = KarenKrill.UniCore.Movement.CameraType.FirstPerson;
        }
        else
        {
            _fpsCamera.gameObject.SetActive(false);
            _thirdPersonCamera.gameObject.SetActive(true);
            cameraType = KarenKrill.UniCore.Movement.CameraType.ThirdPerson;
        }
        _characterMoveBehaviour.CameraType = cameraType;
    }
}
