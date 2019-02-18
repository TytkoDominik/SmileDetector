using DependencyInjection;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace UiElements
{
    public class MiniatureController : MonoBehaviour
    {
        [SerializeField] private Image _smileImage;
        [SerializeField] private Sprite _smile;
        [SerializeField] private Sprite _noSmile;
        [SerializeField] private float _showAnimationTime;
        [SerializeField] private float _fillTime = 0.4f;
        [Inject] private SignalBus _signalBus;

        [Inject]
        private void Initialize()
        {
            _signalBus.Subscribe<PhotoAnalyzedSignal>(UpdatePhoto);
            _signalBus.Subscribe<UpdateMiniatureSignal>(UpdatePhoto);
            ProgressBarSequence();
        }

        private void UpdatePhoto(PhotoAnalyzedSignal signal)
        {
            UpdatePhoto(signal.PhotoName, signal.IsAnybodySmiling);
        }

        private void UpdatePhoto(UpdateMiniatureSignal signal)
        {
            UpdatePhoto(signal.PhotoName, signal.IsAnybodySmiling);
        }

        private void UpdatePhoto(string photoName, bool isAnybodySmiling)
        {
            if (photoName == name)
            {
                SetPhotoSmileStatus(isAnybodySmiling);
            }
        }

        private void SetPhotoSmileStatus(bool isAnybodySmiling)
        {
            _smileImage.DOKill();
            _smileImage.type = Image.Type.Simple;
            _smileImage.color = Color.white;
            
            _smileImage.sprite = isAnybodySmiling ? _smile : _noSmile;
            _smileImage.gameObject.SetActive(true);
            
            Sequence seq = DOTween.Sequence();
            
            seq.Append(_smileImage.transform.DOScale(Vector3.one * 1.3f, _showAnimationTime / 2));
            seq.Append(_smileImage.transform.DOScale(Vector3.one, _showAnimationTime / 2));
            seq.Play();
        }
        
        private void ProgressBarSequence()
        {
            _smileImage.type = Image.Type.Filled;
            _smileImage.fillAmount = 0f;
            
            Sequence seq = DOTween.Sequence();
            seq.AppendCallback(() => _smileImage.fillClockwise = true);
            seq.Append(_smileImage.DOFillAmount(1f, _fillTime).SetEase(Ease.InExpo));
            seq.AppendCallback(() => _smileImage.fillClockwise = false);
            seq.Append(_smileImage.DOFillAmount(0f, _fillTime).SetEase(Ease.OutExpo));
            seq.SetLoops(-1);
            seq.Play();
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<PhotoAnalyzedSignal>(UpdatePhoto);
            _signalBus.Unsubscribe<UpdateMiniatureSignal>(UpdatePhoto);
        }
    }
}