using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;
using System.Linq;

public class GridManager : IDisposable, Zenject.IInitializable
{
    private Tile.Factory _tileFactory;
    private Tile[,] _tiles;
    private Vector2 _startPosition;
    private bool _isMoved = true;
    private DG.Tweening.Sequence _tileMovements;
    private SignalBus _signalBus;
    private Settings _settings;

    public GridManager(Settings settings, Tile.Factory tileFactory, SignalBus signalBus)
    {
        _settings = settings;
        _tileFactory = tileFactory;
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        NumberRenderer.SetTilesCount(_settings.Width * _settings.Height);
        CalculateStartingPosition();
        InitializeGrid();

        _signalBus.Subscribe<SwipeSignal>(x => OnSwipe(x.Direction));
    }

    public void Dispose()
    {
        _signalBus.TryUnsubscribe<SwipeSignal>(x => OnSwipe(x.Direction));
    }

    private void GameOverCheck()
    {
        for(int y = 0; y < _settings.Height; y++)
        {
            for(int x = 0; x < _settings.Width; x++)
            {
                bool isGameOver = true;
                int currentNumber = _tiles[x, y].Value;

                if(y != _settings.Height - 1 && currentNumber == _tiles[x, y + 1].Value)
                {
                    isGameOver = false;
                }

                if(x != _settings.Width - 1 && currentNumber == _tiles[x + 1, y].Value)
                {
                    isGameOver = false;
                }

                if(!isGameOver)
                {
                    return;
                }
            }
        }

        _signalBus.Fire<GameOverSignal>();
    }

    private void AddScore(int value)
    {
        _signalBus.Fire(new ScoreChangedSignal() { Score = value });
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

        for(int y = 0; y < _settings.Height; y++)
        {
            for(int x = 1; x < _settings.Width; x++)
            {
                if(!_tiles[x, y].IsEmpty)
                {
                    for(posibleTilePos = x - 1; posibleTilePos > 0 && _tiles[posibleTilePos, y].IsEmpty; posibleTilePos--)
                    {}

                    if(posibleTilePos == _settings.Width || !(_tiles[posibleTilePos, y].IsEmpty || _tiles[posibleTilePos, y].Value == _tiles[x, y].Value))
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

        for(int y = 0; y < _settings.Height; y++)
        {
            for(int x = _settings.Width - 1; x >= 0; x--)
            {
                if(!_tiles[x, y].IsEmpty)
                {
                    for(posibleTilePos = x + 1; posibleTilePos < _settings.Width - 1 && _tiles[posibleTilePos, y].IsEmpty; posibleTilePos++)
                    {

                    }

                    if(posibleTilePos == _settings.Width || !(_tiles[posibleTilePos, y].IsEmpty || _tiles[posibleTilePos, y].Value == _tiles[x, y].Value))
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

        for(int x = 0; x < _settings.Width; x++)
        {
            for(int y = _settings.Width - 1; y >= 0; y--)
            {
                if(!_tiles[x, y].IsEmpty)
                {
                    for(posibleTilePos = y + 1; posibleTilePos < _settings.Height - 1 && _tiles[x, posibleTilePos].IsEmpty; posibleTilePos++)
                    {}

                    if(posibleTilePos == _settings.Height || !(_tiles[x, posibleTilePos].IsEmpty || _tiles[x, posibleTilePos].Value == _tiles[x, y].Value))
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

        for(int x = 0; x < _settings.Width; x++)
        {
            for(int y = 1; y < _settings.Height; y++)
            {
                if(!_tiles[x, y].IsEmpty)
                {
                    for(posibleTilePos = y - 1; posibleTilePos > 0 && _tiles[x, posibleTilePos].IsEmpty; posibleTilePos--)
                    {}

                    if(posibleTilePos == _settings.Height || !(_tiles[x, posibleTilePos].IsEmpty || _tiles[x, posibleTilePos].Value == _tiles[x, y].Value))
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

    private void CalculateStartingPosition()
    {
        _startPosition = new Vector2(-CalculaStartCoordinate(_settings.Width), -CalculaStartCoordinate(_settings.Height)); 
    }

    float CalculaStartCoordinate(int tileCount)
    {
        return _settings.TileSpacing * tileCount / 2f - (_settings.TileSpacing - 1) / 2f - 0.5f;
    }

    private void InitializeGrid()
    {
        _tiles = new Tile[_settings.Width, _settings.Height];

        for(int x = 0; x < _settings.Width; x++)
        {
            for(int y = 0; y < _settings.Height; y++)
            {
                Vector2 tilePos = new Vector2(x * _settings.TileSpacing + _startPosition.x, y * _settings.TileSpacing + _startPosition.y);
                _tiles[x, y] = _tileFactory.Create(tilePos);
            }
        }

        AddRandomTile();
        AddRandomTile();
        RenderGrid();
    }

    public void ResetGrid()
    {
        for(int y = 0; y < _settings.Height; y++)
        {
            for(int x = 0; x < _settings.Width; x++)
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
        for(int x = 0; x < _settings.Width; x++)
        {
            for(int y = 0; y < _settings.Height; y++)
            {
                _tiles[x, y].RenderTile();
            }
        }
    }

    private void AddRandomTile()
    {
        List<Vector2Int> emptyCells = new List<Vector2Int>();

        for(int i = 0; i < _settings.Width; i++)
        {
            for(int j = 0; j < _settings.Height; j++)
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

    [Serializable]
    public class Settings
    {
        public int Width, Height;
        public float TileSpacing;
    }
}