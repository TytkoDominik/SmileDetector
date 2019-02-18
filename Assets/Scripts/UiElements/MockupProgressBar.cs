using DependencyInjection;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UiElements
{
    public class MockupProgressBar : MonoBehaviour
    {
        private const float FillTime = 0.4f;
        private const float DelayTime = 0.1f;
        private Image _progressBar;
        
        [Inject]
        private void Initialize(SignalBus signalBus)
        {
            _progressBar = GetComponent<Image>();
            signalBus.Subscribe<PhotoTakenSignal>(ProgressBarSequence2);
        }

        private void ProgressBarSequence2()
        {
            Sequence seq = DOTween.Sequence();
            seq.AppendCallback(() => _progressBar.fillClockwise = true);
            seq.Append(_progressBar.DOFade(0.8f, FillTime).SetEase(Ease.InExpo));
            seq.AppendCallback(() => _progressBar.fillClockwise = false);
            seq.Append(_progressBar.DOFade(0f, FillTime).SetEase(Ease.OutExpo));
            seq.Play(); 
        }

        
    }
}