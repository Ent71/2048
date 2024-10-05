using System;
using UnityEngine;
using Zenject;

public class Game3DInstaller : MonoInstaller
{
    [Inject]
    private Settings _settings;

    public override void InstallBindings()
    {
        // Container.BindInterfacesTo<Cube>().AsSingle();
        // Container.Bind<FireControls>().FromNew().AsSingle(); 

        // Container.BindFactory<Cube, Cube.Factory>().FromComponentInNewPrefab(_settings.CubePrefab).UnderTransformGroup("Cubes").AsSingle(); // TODO: pool
        Container.BindFactory<Cube, Cube.Factory>().FromMonoPoolableMemoryPool(
            x => x.WithInitialSize(17).FromComponentInNewPrefab(_settings.CubePrefab).UnderTransformGroup("Cubes"));
        Container.BindFactory<Effect, Effect.Factory>().FromMonoPoolableMemoryPool(
            x => x.WithInitialSize(20).FromComponentInNewPrefab(_settings.CollisionEffectPrefab).UnderTransformGroup("CollisionEffects"));
        // Container.BindFactory<Effect, Effect.Factory>().FromComponentInNewPrefab(_settings.CollisionEffectPrefab).UnderTransformGroup("CollisionEffects").AsSingle();
        
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