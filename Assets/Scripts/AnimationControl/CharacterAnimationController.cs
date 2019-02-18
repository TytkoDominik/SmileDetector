using DependencyInjection;
using UnityEngine;
using Zenject;

namespace AnimationControl
{
    public class CharacterAnimationController : MonoBehaviour
    {
        private Animator _animator;
        
        [Inject]
        private void Initialize(SignalBus signalBus)
        {
            _animator = GetComponent<Animator>();
            
            signalBus.Subscribe<PhotoAnalyzedSignal>(s =>
            {
                _animator.SetTrigger(s.IsAnybodySmiling ? "Laugh" : "Stand");
            });
        }

    }
}