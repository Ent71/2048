using System;
using UnityEngine;
using Zenject;

public class Game3DInstaller : MonoInstaller
{
    [Inject]
    private Settings _settings;

    public override void InstallBindings()
    {
        Container.BindFactory<Cube, Cube.Factory>().FromMonoPoolableMemoryPool(
            x => x.WithInitialSize(17).FromComponentInNewPrefab(_settings.CubePrefab).UnderTransformGroup("Cubes"));
        Container.BindFactory<Effect, Effect.Factory>().FromMonoPoolableMemoryPool(
            x => x.WithInitialSize(20).FromComponentInNewPrefab(_settings.CollisionEffectPrefab).UnderTransformGroup("CollisionEffects"));
        
        Container.Bind<TrailEffect>().FromComponentInChildren().WhenInjectedInto<Cube>();
        Container.Bind<MergeEffect>().FromComponentInChildren().WhenInjectedInto<Cube>();
        Container.Bind<Canvas>().FromComponentInChildren().WhenInjectedInto<Cube>();
        Container.BindInstance<string>("Score3D").WhenInjectedInto<Score>();
    }

    [Serializable]
    public class Settings
    {
        public Cube CubePrefab;
        public CollisionEffect CollisionEffectPrefab;
    }
}