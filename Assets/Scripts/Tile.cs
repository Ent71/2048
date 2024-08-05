using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class Tile : MonoBehaviour
{
    [SerializeField] private NumberRenderer _numberRenderer;
    [SerializeField] private TileMovement _tileMovement;

    private int _value = 0;
    private int _powerOfTwo = 0;
    
    private bool _isEmpty = true;

    public bool IsEmpty => _isEmpty;
    public int Value => _value;
    public int PowerOfTwo => _powerOfTwo;

    public void SetTwo()
    {
        _value = 2;
        _powerOfTwo = 1;
        _isEmpty = false;
        _numberRenderer.ChangeColor(_powerOfTwo);
    }

    public void RemoveNumber()
    {
        _value = 0;
        _powerOfTwo = 0;
        _isEmpty = true;
    }

    public void MultiplyByTwo()
    {
        _value *= 2;
        _powerOfTwo++;
        _numberRenderer.ChangeColor(_powerOfTwo);
    }

    public void SpawnNumber(Tile _tile)
    {
        _isEmpty = false;
        _value = _tile.Value;
        _powerOfTwo = _tile.PowerOfTwo;
        _numberRenderer.ChangeColor(_powerOfTwo);
    }

    public void RenderTile()
    {
        _numberRenderer.RenderNumber(_value);
    }

    public Tween MoveTile(Vector3 target)
    {
        return _tileMovement.MoveTile(target);
    }
}
