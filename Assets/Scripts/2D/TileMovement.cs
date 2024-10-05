using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using Zenject;
using System;

public class TileMovement : MonoBehaviour
{
    private Settings _settings;
    private Tween _tween;
    private Vector3 _primaryPosition;
    public event UnityAction MovementEnd;

    [Inject]
    private void Construct(Settings settings)
    {
        _settings = settings;
    }


    public Tween MoveTile(Vector3 targetPosition)
    {
        _tween = gameObject.transform.DOMove(targetPosition, _settings.Duration);
        _tween.OnComplete(ResetPosition);

        return _tween;
    }

    private void Start()
    {
        _primaryPosition = transform.position;
    }

    public void ResetPosition()
    {
        gameObject.transform.position = _primaryPosition;
        MovementEnd.Invoke();
    }

    [Serializable]
    public class Settings
    {
        public float Duration;
    }
}
