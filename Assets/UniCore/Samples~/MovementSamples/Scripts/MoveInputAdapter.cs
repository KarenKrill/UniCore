using KarenKrill.UniCore.Movement;
using UnityEngine;

public class MoveInputAdapter : MonoBehaviour
{
    [SerializeField]
    private InputController _inputController;
    [SerializeField]
    private InputCharacterMoveContoller _inputCharacterMoveContoller;

    private void Awake()
    {
        _inputCharacterMoveContoller.Initialize(_inputController);
    }

    private void OnEnable()
    {
        _inputCharacterMoveContoller.enabled = true;
    }

    private void OnDisable()
    {
        _inputCharacterMoveContoller.enabled = false;
    }
}
