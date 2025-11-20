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

        public virtual void Show(bool smoothly = true)
        {
            // No need smooth showing if duration is zero or application is quitting
            if (smoothly && !_isQuiting && _fadeInDuration > float.Epsilon)
            {
                // Start showing only if not already showing and if not fully visible
                if (_showCts is null && (!gameObject.activeInHierarchy || _canvasGroup.alpha + float.Epsilon < 1f))
                {
                    _closeCts?.Cancel();
                    _showCts = new();
                    var duration = _fadeInDuration;
                    if (gameObject.activeInHierarchy)
                    {
                        if (_canvasGroup.alpha > float.Epsilon)
                        { // Already partially visible
                            // Adjust duration based on current alpha
                            duration = Mathf.Lerp(0, duration, 1 - _canvasGroup.alpha);
                        }
                    }
                    else if (!_isQuiting)
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
                OnShow();
            }
        }
        public virtual void Close(bool smoothly = true)
        {
            // No need smooth closing if duration is zero or application is quitting
            if (smoothly && !_isQuiting && _fadeOutDuration > float.Epsilon)
            {
                // Start closing only if not already closing and if currently active
                if (_closeCts is null && gameObject.activeInHierarchy)
                {
                    _showCts?.Cancel();
                    _closeCts = new();
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
        private float _fadeInDuration = .5f;
        [SerializeField]
        private float _fadeOutDuration = .5f;
        private CancellationTokenSource _showCts = null;
        private CancellationTokenSource _closeCts = null;
        private bool _isQuiting = false;

        private void OnApplicationQuit()
        {
            _isQuiting = true;
            _showCts?.Cancel();
            _closeCts?.Cancel();
        }
        private void OnShow()
        {
            if (!_isQuiting)
            {
                gameObject.SetActive(true);
                SetFocus(true);
            }
        }
        private void OnClose()
        {
            if (!_isQuiting)
            {
                SetFocus(false);
                gameObject.SetActive(false);
            }
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
                if (!_isQuiting)
                {
                    _canvasGroup.alpha = Mathf.Lerp(start, end, progress);
                    elapsedTime += Time.unscaledDeltaTime;
                    try
                    {
                        await UniTask.Yield(cancellationToken);
                    }
                    catch (System.OperationCanceledException) { }
                }
            }
            if (!_isQuiting)
            {
                _canvasGroup.alpha = end;
            }
        }
        private async UniTask CloseSmoothlyAsync(CancellationToken cancellationToken)
        {
            float elapsedTime = 0f;
            float progress = 0f;
#if !UNITY_WEBGL
            await UniTask.SwitchToMainThread();
#endif
            while (progress < 1f && !cancellationToken.IsCancellationRequested)
            {
                progress = elapsedTime / _fadeOutDuration;
                if (!_isQuiting)
                {
                    _canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);
                    elapsedTime += Time.unscaledDeltaTime;
                    await UniTask.Yield();
                }
            }
            if (!_isQuiting)
            {
                OnClose();
            }
        }
    }
}