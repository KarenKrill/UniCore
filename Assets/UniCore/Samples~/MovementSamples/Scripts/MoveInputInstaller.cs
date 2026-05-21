using KarenKrill.UniCore.Movement;
using UnityEngine;

[RequireComponent(typeof(InputController), typeof(MoveInputContextProvider))]
public class MoveInputInstaller : MonoBehaviour
{
    [SerializeField]
    private InputController _inputController;
    [SerializeField]
    private MoveInputContextProvider _moveInputContextProvider;

    private void Awake()
    {
        _moveInputContextProvider.Initialize(_inputController);
    }

    private void OnEnable()
    {
        _moveInputContextProvider.enabled = true;
    }

    private void OnDisable()
    {
        _moveInputContextProvider.enabled = false;
    }
}
