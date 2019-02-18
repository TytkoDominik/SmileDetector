using Core;
using Zenject;

namespace DependencyInjection
{
    public class SceneContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<PhotoDataSignal>();
            Container.DeclareSignal<PhotoTakenSignal>();
            Container.DeclareSignal<SwitchCameraSignal>();
            Container.DeclareSignal<ClearAllDataSignal>();
            Container.DeclareSignal<PhotoAnalyzedSignal>();
            Container.DeclareSignal<PhotoSerializedSignal>();
            Container.DeclareSignal<UpdateMiniatureSignal>();
            Container.DeclareSignal<MiniaturesPreparedSignal>();

            Container.Bind<FaceAnalyzer>().ToSelf().FromNew().AsSingle().NonLazy();
            Container.Bind<DataSerializer>().ToSelf().FromNew().AsSingle().NonLazy();
        }
    }
}