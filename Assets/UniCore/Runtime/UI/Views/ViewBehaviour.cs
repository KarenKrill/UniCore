#nullable enable

using System.Threading;

using UnityEngine.EventSystems;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace KarenKrill.UniCore.UI.Views
{
    using Abstractions;

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ViewBehaviour : MonoBehaviour, IView
    {
        public bool Interactable { get => _canvasGroup.interactable; set => _canvasGroup.interactable = value; }

        public virtual void Show(bool smoothly = true)
        {
            OnShow();
            if (smoothly)
            {
                _canvasGroup.alpha = 0;
                if (_showCts is null)
                {
                    _closeCts?.Cancel();
                    _showCts = new();
                    ShowSmoothlyAsync(_showCts.Token).ContinueWith(() =>
                    {
                        _showCts?.Dispose();
                        _showCts = null;
                    }).Forget();
                }
            }
        }
        public virtual void Close(bool smoothly = true)
        {
            if (smoothly)
            {
                if (_closeCts is null)
                {
                    _showCts?.Cancel();
                    _closeCts = new();
                    CloseSmoothlyAsync(_closeCts.Token).ContinueWith(() =>
                    {
                        _closeCts?.Dispose();
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
        private float _fadeDuration = 1f;
        private CanvasGroup _canvasGroup;
        private CancellationTokenSource? _showCts = null;
        private CancellationTokenSource? _closeCts = null;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
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
        private async UniTask ShowSmoothlyAsync(CancellationToken cancellationToken)
        {
            float elapsedTime = 0f;
            float progress = 0f;
            while (progress < 1f && !cancellationToken.IsCancellationRequested)
            {
                progress = elapsedTime / _fadeDuration;
#if !UNITY_WEBGL
                await UniTask.SwitchToMainThread();
#endif
                _canvasGroup.alpha = Mathf.Clamp01(progress);
                elapsedTime += Time.unscaledDeltaTime;
#if !UNITY_WEBGL
                await UniTask.SwitchToThreadPool();
#endif
                await UniTask.Yield();
            }
        }
        private async UniTask CloseSmoothlyAsync(CancellationToken cancellationToken)
        {
            float elapsedTime = 0f;
            float progress = 0f;
            while (progress < 1f && !cancellationToken.IsCancellationRequested)
            {
                progress = elapsedTime / _fadeDuration;
#if !UNITY_WEBGL
                await UniTask.SwitchToMainThread();
#endif
                _canvasGroup.alpha = 1 - Mathf.Clamp01(progress);
                elapsedTime += Time.unscaledDeltaTime;
#if !UNITY_WEBGL
                await UniTask.SwitchToThreadPool();
#endif
                await UniTask.Yield();
            }
#if !UNITY_WEBGL
            await UniTask.SwitchToThreadPool();
#endif
            OnClose();
        }
    }
}