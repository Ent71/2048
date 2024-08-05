using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DynamicCameraWidthScaling : MonoBehaviour
{
    [SerializeField] private float sizeInMeters = 5f;
    private Camera _camera;

    private void Start()
    {
        _camera = gameObject.GetComponent<Camera>();
        SetCameraSize(_camera);
    }

    private void SetCameraSize(Camera camera)
    {
        float orthoSize = sizeInMeters * Screen.height / Screen.width * 0.5f;
        Camera.main.orthographicSize = orthoSize;
    }
}
