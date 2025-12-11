using System.Threading;

using UnityEngine;
using UnityEngine.EventSystems;

using Cysharp.Threading.Tasks;

namespace KarenKrill.UniCore.UI.Views
{
    using Abstractions;

    public abstract class ViewBehaviour : MonoBehaviour, IView
    {
        public bool Interactable { get => _canvasGroup.interactable; set => _canvasGroup.interactable = value; }
        public int SortOrder => _sortOrder;

        public virtual void Show(bool smoothly = true)
        {
            // No need smooth showing if duration is zero
            if (smoothly && _fadeInDuration > float.Epsilon)
            {
                // Start showing only if not already showing and if not fully visible
                if (_showCts is null && (!gameObject.activeInHierarchy || _canvasGroup.alpha + float.Epsilon < 1f))
                {
                    _closeCts?.Cancel();
                    _showCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, Application.exitCancellationToken);
                    var duration = _fadeInDuration;
                    if (gameObject.activeInHierarchy)
                    {
                        if (_canvasGroup.alpha > float.Epsilon)
                        { // Already partially visible
                            // Adjust duration based on current alpha
                            duration = Mathf.Lerp(0, duration, 1 - _canvasGroup.alpha);
                        }
                    }
                    else
                    {
                        // Starting from invisible if not active
                        _canvasGroup.alpha = 0;
                    }
                    OnShow();
                    SmoothlyAlphaTransitionAsync(_canvasGroup.alpha, 1f, duration, _showCts.Token).ContinueWith(() =>
                    {
                        _showCts.Dispose();
                        _showCts = null;
                    }).Forget();
                }
            }
            else
            {
                _closeCts?.Cancel();
                OnShow();
            }
        }
        public virtual void Close(bool smoothly = true)
        {
            // No need smooth closing if duration is zero
            if (smoothly && _fadeOutDuration > float.Epsilon)
            {
                // Start closing only if not already closing and if currently active
                if (_closeCts is null && gameObject.activeInHierarchy)
                {
                    _showCts?.Cancel();
                    _closeCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, Application.exitCancellationToken);
                    var duration = _fadeOutDuration;
                    if (Mathf.Abs(1f - _canvasGroup.alpha) > float.Epsilon)
                    { // Already partially invisible
                        // Adjust duration based on current alpha
                        duration = Mathf.Lerp(0, duration, _canvasGroup.alpha);
                    }
                    SmoothlyAlphaTransitionAsync(_canvasGroup.alpha, 0, duration, _closeCts.Token).ContinueWith(() =>
                    {
                        if (!_closeCts.IsCancellationRequested)
                        {
                            OnClose();
                        }
                        _closeCts.Dispose();
                        _closeCts = null;
                    }).Forget();
                }
            }
            else
            {
                _showCts?.Cancel();
                OnClose();
            }
        }
        public virtual void SetFocus(bool isFocused)
        {
            if (EventSystem.current != null)
            {
                GameObject sellectedObject = isFocused ? DefaultFocusObject : null;
                EventSystem.current.SetSelectedGameObject(sellectedObject);
            }
        }

        [SerializeField]
        protected GameObject DefaultFocusObject;

        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private int _sortOrder = 0;
        [SerializeField]
        private float _fadeInDuration = .5f;
        [SerializeField]
        private float _fadeOutDuration = .5f;
        private CancellationTokenSource _showCts = null;
        private CancellationTokenSource _closeCts = null;

        private void OnShow()
        {
            gameObject.SetActive(true);
            SetFocus(true);
        }
        private void OnClose()
        {
            SetFocus(false);
            gameObject.SetActive(false);
        }
        private async UniTask SmoothlyAlphaTransitionAsync(float start, float end, float duration, CancellationToken cancellationToken)
        {
            float elapsedTime = 0f;
            float progress = 0f;
#if !UNITY_WEBGL
            await UniTask.SwitchToMainThread();
#endif
            while (progress < 1f && !cancellationToken.IsCancellationRequested)
            {
                progress = elapsedTime / duration;
                _canvasGroup.alpha = Mathf.Lerp(start, end, progress);
                elapsedTime += Time.unscaledDeltaTime;
                try
                {
                    await UniTask.Yield(cancellationToken);
                }
                catch (System.OperationCanceledException) { }
            }
            if (!cancellationToken.IsCancellationRequested)
            {
                _canvasGroup.alpha = end;
            }
            else
            {
                bool isHiding = end < start;
                var taskType = isHiding ? "hiding" : "showing";
                Debug.LogWarning($"Smooth {taskType} canceled!");
            }
        }
    }
}