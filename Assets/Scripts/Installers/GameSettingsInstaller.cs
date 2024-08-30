using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    public DynamicCameraWidthScaling.Settings DynamicCameraWidthScaling;
    public GridManager.Settings GridManager;
    public SwipeDetection.Settings SwipeDetection;
    public GameInstaller.Settings GameInstaller;
    public NumberRenderer.Settings NumberRenderer;
    public TileMovement.Settings TileMovement;

    public override void InstallBindings()
    {
        Container.BindInstance(DynamicCameraWidthScaling).IfNotBound();
        Container.BindInstance(GridManager).IfNotBound();
        Container.BindInstance(SwipeDetection).IfNotBound();
        Container.BindInstance(GameInstaller).IfNotBound();
        Container.BindInstance(TileMovement).IfNotBound();
        Container.BindInstance(NumberRenderer).IfNotBound();
    }
}