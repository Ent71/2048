using System;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Inject]
    private Settings _settings;

    public override void InstallBindings()
    {
        InstallGridManager();
    }

    private void InstallGridManager()
    {
        Container.BindInterfacesAndSelfTo<GridManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<SwipeDetection>().AsSingle().NonLazy();

        Container.BindFactory<Vector2, Tile, Tile.Factory>().FromComponentInNewPrefab(_settings.TilePrefab).UnderTransformGroup("Tiles");

        Container.Bind<TileMovement>().FromComponentInChildren().WhenInjectedInto<Tile>();
        Container.Bind<NumberRenderer>().FromComponentInChildren().WhenInjectedInto<Tile>();
    }

    [Serializable]
    public class Settings
    {
        public GameObject TilePrefab;
    }
}