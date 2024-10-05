using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettings3DInstaller", menuName = "Installers/GameSettings3DInstaller")]
public class GameSettings3DInstaller : ScriptableObjectInstaller<GameSettings3DInstaller>
{
    public Game3DInstaller.Settings GameInstaller;
    public FireManager.Settings FireConfigurations;
    public Cube.Settings MergeSettings;

    public override void InstallBindings()
    {
        Container.BindInstance(GameInstaller).IfNotBound();
        Container.BindInstance(FireConfigurations).IfNotBound();
        Container.BindInstance(MergeSettings).IfNotBound();
    }
}