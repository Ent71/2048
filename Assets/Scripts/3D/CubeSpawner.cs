using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CubeSpawner : MonoBehaviour
{
    // [SerializeField] private List<SpawnPoint> _startPositions;
    private SpawnPoint[] _startPositions;
    private Cube.Factory _cubeFactory;
    private SignalBus _signalBus;

    [Inject]
    private void Construct(Cube.Factory cubeFactory, SignalBus signalBus)
    {
        _cubeFactory = cubeFactory;
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
        SpawnCubes();
    }

    private void SpawnCubes()
    {
        int maxIndex = _startPositions.Length - 1;
        int num1 = Random.Range(0, maxIndex);
        int num2;

        do
        {
            num2 = Random.Range(0, maxIndex);
        } while (num2 == num1);

        _cubeFactory.Create().transform.position = _startPositions[num1].GetPosition();
        _cubeFactory.Create().transform.position = _startPositions[num2].GetPosition();
    }

    private void Awake()
    {
        _startPositions = GetComponentsInChildren<SpawnPoint>();
    }

    private void Start()
    {
        SpawnCubes();
    }
}
