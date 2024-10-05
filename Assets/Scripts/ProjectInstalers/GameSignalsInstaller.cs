using Zenject;

public class GameSignalsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<GameOverSignal>();
        Container.DeclareSignal<ScoreChangedSignal>();
        Container.DeclareSignal<RestartPressedSignal>();
        Container.DeclareSignal<SwipeSignal>();
        Container.DeclareSignal<CubeCountChangedSignal>();
    }
}