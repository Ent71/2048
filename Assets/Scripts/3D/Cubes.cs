using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Cubes : MonoBehaviour
{
    private SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void OnEnable()
    {
        _signalBus.Subscribe<RestartPressedSignal>(OnRestartSignal);
    }

    private void OnDisable()
    {
        _signalBus.Unsubscribe<RestartPressedSignal>(OnRestartSignal);
    }

    private void OnRestartSignal()
    {
        Cube[] _cubes = gameObject.GetComponentsInChildren<Cube>();

        foreach (Cube cube in _cubes)
        {
            Destroy(cube.gameObject);
        }
    }
}
