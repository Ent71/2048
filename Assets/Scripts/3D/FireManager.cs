using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
// using System.Numerics;
using UnityEngine;
using Zenject;

public class FireManager : MonoBehaviour
{
    [SerializeField] private Transform _leftFireLimit;
    [SerializeField] private Transform _rightFireLimit;
    [SerializeField] private Transform _startPoint;

    private SwipeControl _swipeControls;
    private Cube.Factory _cubeFactory;
    private Cube _currentCube;
    private Settings _settings;
    private float _elapsedTimeAfterFire = 0f;
    private float _currentLerp = 0.5f; // TODO: Add default value in settings
    private bool _isReadyToFire = true;
    private bool _isOnPress = false;
    private float _pressPreviousPosition;
    private SignalBus _signalBus;

    [Inject]
    private void Construct(Cube.Factory cubeFactory, Settings settings, SwipeControl swipeControls, SignalBus signalBus)
    {
        _cubeFactory = cubeFactory;
        _settings = settings;
        _swipeControls = swipeControls;
        _signalBus = signalBus;
    }

    private void Start()
    {
        GenerateCube();
        
        _swipeControls.Enable();
    }

    private void OnEnable()
    {
        _swipeControls.SwipeDetection.Press.performed += ctx => PressBegin();
        _swipeControls.SwipeDetection.Press.canceled += ctx => OnFire();

        _signalBus.Subscribe<RestartPressedSignal>(OnRestartSignal);
    }

    private void OnDisable()
    {
        _swipeControls.SwipeDetection.Press.performed -= ctx => PressBegin();
        _swipeControls.SwipeDetection.Press.canceled -= ctx => OnFire();

        _signalBus.Unsubscribe<RestartPressedSignal>(OnRestartSignal);
    }

    private void Update()
    {
        // Debug.Log(_swipeControls.SwipeDetection.Press.IsPressed());
        if(Time.timeScale == 0) // TODO: rewrite using signals
        {
            return;
        }

        if(_isOnPress)
        {
            float newPressPosition = _swipeControls.SwipeDetection.Position.ReadValue<Vector2>().x;
            if(_pressPreviousPosition != newPressPosition && _currentCube != null)
            {
                float sign = _pressPreviousPosition > newPressPosition ? -1 : 1;
                _currentLerp += sign * MathF.Abs((_pressPreviousPosition - newPressPosition) / (float)Screen.width);
                // Debug.Log($"press positions: prev: {_pressPreviousPosition} now: {newPressPosition}, width: {Screen.width}, currentLerp = {_currentLerp}, Mathf: {MathF.Abs((_pressPreviousPosition - newPressPosition) / (float)Screen.width)}");
                
                if(_currentLerp > 1f)
                {
                    _currentLerp = 1f;
                }

                if(_currentLerp < 0f)
                {
                    _currentLerp = 0f;
                }

                _pressPreviousPosition = newPressPosition;
                _currentCube.transform.position = Vector3.Lerp(_leftFireLimit.position, _rightFireLimit.position, _currentLerp);
            }
        }

        if(!_isReadyToFire)
        {
            _elapsedTimeAfterFire += Time.deltaTime;
            if(_elapsedTimeAfterFire >= _settings.TimeBetweenFire)
            {
                _elapsedTimeAfterFire = 0;
                _isReadyToFire = true;
                GenerateCube();
            }
        }
    }

    private void PressBegin()
    {
        Debug.Log("begin");
        Vector2 pressPosiotion = _swipeControls.SwipeDetection.Position.ReadValue<Vector2>();

        if(pressPosiotion.y < Screen.height / 2)
        {
            _isOnPress = true;
            _pressPreviousPosition = pressPosiotion.x;
        }
    }

    private void OnFire()
    {
        Debug.Log("end");
        // if(Time.timeScale == 0) // TODO: rewrite using signals
        // {
        //     return;
        // }
        if(!_isOnPress || _currentCube == null)
        {
            return;
        }

        _isOnPress = false;

        if(_isReadyToFire)
        {
            _currentCube.FireCube(_settings.FireDirection * _settings.FireStrength);
            _currentLerp = 0.5f;
            _isReadyToFire = false;
            _currentCube = null;
        }
        
    }

    private void GenerateCube()
    {
        if(_settings.MaximumCubeCount < Cube.CubeCount)
        {
            _signalBus.Fire<GameOverSignal>();
        }

        _currentCube = _cubeFactory.Create();
        _currentCube.DisableGravity();
        _currentCube.transform.position = _startPoint.position;
    }

    private void OnRestartSignal()
    {
        _isReadyToFire = false;

        if(_currentCube != null)
        {
            // Destroy(_currentCube.gameObject); // TODO: poool
            _currentCube.EnableGravity();
            _currentCube.Dispose();
            _currentCube = null;
        }

        _elapsedTimeAfterFire = 0f;
        GenerateCube();
        _isReadyToFire = true;
    }

    [Serializable]
    public class Settings
    {
        public float FireStrength;
        public Vector3 FireDirection;
        public float TimeBetweenFire;
        public int MaximumCubeCount;
    }
}