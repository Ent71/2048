using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class CubeCount : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private SignalBus _signalBus;

    [Inject]
    private void Init(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void OnEnable()
    {
        _signalBus.Subscribe<RestartPressedSignal>(OnRestartPressed);
        _signalBus.Subscribe<CubeCountChangedSignal>(OnCubeCountChange);
    }

    private void OnDisable()
    {
        _signalBus.Unsubscribe<RestartPressedSignal>(OnRestartPressed);
        _signalBus.Unsubscribe<CubeCountChangedSignal>(OnCubeCountChange);
    }

    private void OnRestartPressed()
    {
        Cube.ResetCubeCount();
    }

    private void OnCubeCountChange(CubeCountChangedSignal signal)
    {
        _text.text = signal.Count.ToString();
    }
}
