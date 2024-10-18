using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Camera))]
public class DynamicCameraWidthScaling : MonoBehaviour
{
    private Settings _settings;
    private Camera _camera;
    
    [Inject]
    private void Construct(Settings settings)
    {
        _settings = settings;
        _camera = gameObject.GetComponent<Camera>();
    }

    private void Start()
    {
        SetCameraSize(_camera);
    }

    private void SetCameraSize(Camera camera)
    {
        float orthoSize = _settings.sizeInMeters * Screen.height / Screen.width * 0.5f;
        Camera.main.orthographicSize = orthoSize;
    }

    [Serializable]
    public class Settings
    {
        public float sizeInMeters;
    }
}