using System.Collections.Generic;

using UnityEngine;

namespace KarenKrill.UniCore.UI.Views
{
    using Abstractions;

    public class ViewFactory : IViewFactory
    {
        public ViewFactory(GameObject parentGameObject, List<GameObject> viewPrefabs)
        {
            _parentGameObject = parentGameObject;
            _viewPrefabs = viewPrefabs;
        }
        public ViewType Create<ViewType>() where ViewType : IView
        {
            foreach (var viewPrefab in _viewPrefabs)
            {
                if (viewPrefab.TryGetComponent<ViewType>(out _))
                {
                    var viewGameObject = Object.Instantiate(viewPrefab, _parentGameObject.transform);
                    viewGameObject.SetActive(false);
                    viewGameObject.name = viewPrefab.name;
                    var view = viewGameObject.GetComponent<ViewType>();
                    if (!_sortCache.ContainsKey(view.SortOrder))
                    {
                        _sortCache[view.SortOrder] = new();
                    }
                    SortViews();
                    return view;
                }
            }
            throw new System.InvalidOperationException($"There is no prefab for \"{typeof(ViewType).Name}\" view");
        }

        private readonly GameObject _parentGameObject;
        private readonly List<GameObject> _viewPrefabs = new();
        private readonly SortedList<int, List<Transform>> _sortCache = new();

        private void SortViews()
        {
            foreach(var viewTransforms in _sortCache.Values)
            {
                viewTransforms.Clear();
            }
            foreach (Transform child in _parentGameObject.transform)
            {
                if (child.TryGetComponent<IView>(out var view))
                {
                    _sortCache[view.SortOrder].Add(child);
                }
            }
            int index = 0;
            foreach (var sortInfo in _sortCache)
            {
                foreach (var viewTransform in sortInfo.Value)
                {
                    viewTransform.SetSiblingIndex(index++);
                }
            }
        }
    }
}