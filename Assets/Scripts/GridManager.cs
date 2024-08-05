using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private float _tileSpacing;
    [SerializeField] private SwipeDetection _swipeDetection;

    private Tile[,] _tiles;
    private bool _isMoved = true;
    private DG.Tweening.Sequence _tileMovements;

    public event UnityAction<int> ScoreChanged;
    public event UnityAction GameOver;

    private void OnEnable()
    {
        _swipeDetection.Swipe += OnSwipe;
    }

    private void OnDisable()
    {
        _swipeDetection.Swipe -= OnSwipe;
    }

    private void Start()
    {
        NumberRenderer.SetTilesCount(_width * _height);
        InitializeGrid();
    }

    private void GameOverCheck()
    {
        for(int y = 0; y < _height; y++)
        {
            for(int x = 0; x < _width; x++)
            {
                bool isGameOver = true;
                int currentNumber = _tiles[x, y].Value;

                if(y != _height - 1 && currentNumber == _tiles[x, y + 1].Value)
                {
                    isGameOver = false;
                }

                if(x != _width - 1 && currentNumber == _tiles[x + 1, y].Value)
                {
                    isGameOver = false;
                }

                if(!isGameOver)
                {
                    return;
                }
            }
        }

        GameOver?.Invoke();
    }

    private void AddScore(int value)
    {
        ScoreChanged?.Invoke(value);
    }

    private void OnMovementComplete()
    {
        if(_isMoved)
        {
            AddRandomTile();
            RenderGrid();
        }

        _isMoved = false;
    }

    private void OnSwipe(Vector2 direction)
    {
        _tileMovements.Complete(true);
        _tileMovements = DOTween.Sequence();
        _tileMovements.OnComplete(OnMovementComplete);
        
        if(direction == Vector2.left)
        {
            TryMoveLeft();
        }
        else if(direction == Vector2.right)
        {
            TryMoveRight();
        }  
        else if(direction == Vector2.up)
        {
            TryMoveUp();
        }
        else if(direction == Vector2.down)
        {
            TryMoveDown();
        }
    }

    private void TryMoveLeft()
    {
        int posibleTilePos = 1;

        for(int y = 0; y < _height; y++)
        {
            for(int x = 1; x < _width; x++)
            {
                if(!_tiles[x, y].IsEmpty)
                {
                    for(posibleTilePos = x - 1; posibleTilePos > 0 && _tiles[posibleTilePos, y].IsEmpty; posibleTilePos--)
                    {}

                    if(posibleTilePos == _width || !(_tiles[posibleTilePos, y].IsEmpty || _tiles[posibleTilePos, y].Value == _tiles[x, y].Value))
                    {
                        posibleTilePos++;
                    }
                    
                    if(posibleTilePos != x)
                    {
                        MoveTile(_tiles[x, y], _tiles[posibleTilePos, y]);
                    }
                }
            }
        }
    }

    private void TryMoveRight()
    {
        int posibleTilePos = 1;

        for(int y = 0; y < _height; y++)
        {
            for(int x = _width - 1; x >= 0; x--)
            {
                if(!_tiles[x, y].IsEmpty)
                {
                    for(posibleTilePos = x + 1; posibleTilePos < _width - 1 && _tiles[posibleTilePos, y].IsEmpty; posibleTilePos++)
                    {

                    }

                    if(posibleTilePos == _width || !(_tiles[posibleTilePos, y].IsEmpty || _tiles[posibleTilePos, y].Value == _tiles[x, y].Value))
                    {
                        posibleTilePos--;
                    }
                    
                    if(posibleTilePos != x)
                    {
                        MoveTile(_tiles[x, y], _tiles[posibleTilePos, y]);
                    }
                }
            }
        }
    }

    private void TryMoveUp()
    {
        int posibleTilePos = 1;

        for(int x = 0; x < _width; x++)
        {
            for(int y = _width - 1; y >= 0; y--)
            {
                if(!_tiles[x, y].IsEmpty)
                {
                    for(posibleTilePos = y + 1; posibleTilePos < _height - 1 && _tiles[x, posibleTilePos].IsEmpty; posibleTilePos++)
                    {}

                    if(posibleTilePos == _height || !(_tiles[x, posibleTilePos].IsEmpty || _tiles[x, posibleTilePos].Value == _tiles[x, y].Value))
                    {
                        posibleTilePos--;
                    }
                    
                    if(posibleTilePos != y)
                    {
                        MoveTile(_tiles[x, y], _tiles[x, posibleTilePos]);
                    }
                }
            }
        }
    }

    private void TryMoveDown()
    {
        int posibleTilePos = 1;

        for(int x = 0; x < _width; x++)
        {
            for(int y = 1; y < _height; y++)
            {
                if(!_tiles[x, y].IsEmpty)
                {
                    for(posibleTilePos = y - 1; posibleTilePos > 0 && _tiles[x, posibleTilePos].IsEmpty; posibleTilePos--)
                    {}

                    if(posibleTilePos == _height || !(_tiles[x, posibleTilePos].IsEmpty || _tiles[x, posibleTilePos].Value == _tiles[x, y].Value))
                    {
                        posibleTilePos++;
                    }
                    
                    if(posibleTilePos != y)
                    {
                        MoveTile(_tiles[x, y], _tiles[x, posibleTilePos]);
                    }
                }
            }
        }
    }

    private void MoveTile(Tile currentTile, Tile destinationTile)
    {
        _isMoved = true;
        
        if(destinationTile.IsEmpty)
        {
            destinationTile.SpawnNumber(currentTile);
        }
        else
        {
            destinationTile.MultiplyByTwo();
            AddScore(destinationTile.Value);
        }

        Vector3 destinationPosition = new Vector3(destinationTile.transform.position.x, destinationTile.transform.position.y);
        _tileMovements.Join(currentTile.MoveTile(destinationPosition));
        
        currentTile.RemoveNumber();
    }

    private void InitializeGrid()
    {
        Vector2 centerPosition = gameObject.transform.position;

        _tiles = new Tile[_width, _height];

        for(int x = 0; x < _width; x++)
        {
            for(int y = 0; y < _height; y++)
            {
                Vector2 tilePos = new Vector2(x * _tileSpacing + centerPosition.x, y * _tileSpacing + centerPosition.y);
                _tiles[x, y] = Instantiate(_tilePrefab, tilePos, Quaternion.identity, gameObject.transform);
            }
        }

        AddRandomTile();
        AddRandomTile();
        RenderGrid();
    }

    public void ResetGrid()
    {
        for(int y = 0; y < _height; y++)
        {
            for(int x = 0; x < _width; x++)
            {
                if(!_tiles[x, y].IsEmpty)
                {
                    _tiles[x, y].RemoveNumber();
                }
            }
        }

        AddRandomTile();
        AddRandomTile();
        RenderGrid();
    }

    private void RenderGrid()
    {
        for(int x = 0; x < _width; x++)
        {
            for(int y = 0; y < _height; y++)
            {
                _tiles[x, y].RenderTile();
            }
        }
    }

    private void AddRandomTile()
    {
        List<Vector2Int> emptyCells = new List<Vector2Int>();

        for(int i = 0; i < _width; i++)
        {
            for(int j = 0; j < _height; j++)
            {
                if(_tiles[i, j].IsEmpty)
                {
                    emptyCells.Add(new Vector2Int(i, j));
                }
            }
        }

        if(emptyCells.Count > 0)
        {
            Vector2Int randomCell = emptyCells[UnityEngine.Random.Range(0, emptyCells.Count -1)];
            _tiles[randomCell.x, randomCell.y].SetTwo();
        }

        if(emptyCells.Count == 1)
        {
            GameOverCheck();
        }
    }
}