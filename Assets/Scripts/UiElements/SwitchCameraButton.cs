using DependencyInjection;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UiElements
{
    public class SwitchCameraButton : MonoBehaviour
    {
        [Inject]
        private void Initialize(SignalBus signalBus)
        {
            GetComponent<Button>().onClick.AddListener(() => signalBus.Fire<SwitchCameraSignal>());
        }
    }
}