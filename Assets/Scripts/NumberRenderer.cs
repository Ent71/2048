using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(SpriteRenderer))]
public class NumberRenderer : MonoBehaviour
{
    [SerializeField] private TextMesh _text;

    Settings _settings;
    private SpriteRenderer _spriteRenderer;
    private static int _tilesCount = 2;

    [Inject]
    private void Construct(Settings settings)
    {
        _settings = settings;
    }

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
        _spriteRenderer.color = Color.Lerp(_settings.StartColor, _settings.TargetColor, (float)powerOfTwo / _tilesCount);
    }

    [Serializable]
    public class Settings
    {
        public Color StartColor;
        public Color TargetColor;
    }
}