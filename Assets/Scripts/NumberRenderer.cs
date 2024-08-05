using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.SpriteAssetUtilities;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NumberRenderer : MonoBehaviour
{
    [SerializeField] private TextMesh _text;
    [SerializeField] private Color _startColor;
    [SerializeField] private Color _targetColor;

    private SpriteRenderer _spriteRenderer;
    private static int _tilesCount = 2;

    public static void SetTilesCount(int tilesCount)
    {
        _tilesCount = tilesCount;
    }

    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public bool RenderNumber(int value)
    {
        if(value != 0)
        {
            _text.text = value.ToString();
            gameObject.SetActive(true);
            return false;
        }
        else
        {
            gameObject.SetActive(false);
            return true;
        }
    }

    public void ChangeColor(int powerOfTwo)
    {
        _spriteRenderer.color = Color.Lerp(_startColor, _targetColor, (float)powerOfTwo / _tilesCount);
    }
}