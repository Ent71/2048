using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class TileMovement : MonoBehaviour
{
    [SerializeField] float _duration = 1;
    private Tween _tween;
    private Vector3 _primaryPosition;
    public event UnityAction MovementEnd;

    public Tween MoveTile(Vector3 targetPosition)
    {
        _tween = gameObject.transform.DOMove(targetPosition, _duration);
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
}
