using UnityEngine;

public class BoredBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetIdle();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_isBored)
        {
            _idleTime += Time.deltaTime;
            if (_idleTime > _timeUntilBored && stateInfo.normalizedTime % 1 < 0.02f)
            {
                _isBored = true;
                _animationIndex = Random.Range(1, _boredAnimationsCount + 1);
                _animationIndex = _animationIndex * 2 - 1;
                animator.SetFloat(BoredAnimationHash.Value, _animationIndex - 1);
            }
        }
        else if (stateInfo.normalizedTime % 1 > 0.98f) // end of looped animation
        {
            ResetIdle();
        }
        animator.SetFloat(BoredAnimationHash.Value, _animationIndex, 0.2f, Time.deltaTime);
    }

    private static readonly System.Lazy<int> BoredAnimationHash = new(() => Animator.StringToHash("BoredAnimation"));

    [SerializeField]
    private float _timeUntilBored = 3.0f;
    [SerializeField]
    private int _boredAnimationsCount = 2;

    private bool _isBored = false;
    private float _idleTime;
    private int _animationIndex = 0;

    private void ResetIdle()
    {
        if (_isBored)
        {
            _animationIndex--;
            _isBored = false;
        }
        _idleTime = 0.0f;
    }
}
