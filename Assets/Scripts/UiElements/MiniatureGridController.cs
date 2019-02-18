using System.Collections.Generic;
using Core;
using DependencyInjection;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UiElements
{
    public class MiniatureGridController : MonoBehaviour
    {
        [SerializeField] private RawImage _miniaturePrefab;
        [SerializeField] private float _showAnimationTime;
        [Inject] private SignalBus _signalBus;
        [Inject] private DataSerializer _dataSerializer;
        [Inject] private DiContainer _container;
        private List<string> _currentLoadedMiniatureNames = new List<string>();

        [Inject]
        private void Initialize()
        {
            UpdateMiniatureGrid(false);
            _signalBus.Subscribe<PhotoSerializedSignal>(() => UpdateMiniatureGrid());
            _signalBus.Subscribe<ClearAllDataSignal>(ClearAllData);
        }

        private void UpdateMiniatureGrid(bool withAnimation = true)
        {
            foreach (Photo miniature in _dataSerializer.GetNotYetLoadedMiniatures(_currentLoadedMiniatureNames))
            {
                RawImage miniatureImage = _container.InstantiatePrefab(_miniaturePrefab, transform).GetComponent<RawImage>();
                miniatureImage.texture = miniature.Texture;
                miniatureImage.name = miniature.Name;
                miniatureImage.transform.SetAsFirstSibling();

                if (withAnimation)
                {
                    miniatureImage.transform.localScale = Vector3.one * 1.3f;
                    miniatureImage.transform.DOScale(Vector3.one, _showAnimationTime);
                }

                _currentLoadedMiniatureNames.Add(miniature.Name);

                SetGridRectSize();
            }

            _signalBus.Fire<MiniaturesPreparedSignal>();
        }

        private void SetGridRectSize()
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(_currentLoadedMiniatureNames.Count * 120, 160);
        }

        private void ClearAllData()
        {
            foreach (MiniatureController child in transform.GetComponentsInChildren<MiniatureController>())
            {
                Destroy(child.gameObject);
            }
            
            _currentLoadedMiniatureNames.Clear();
            SetGridRectSize();
        }
    }
}