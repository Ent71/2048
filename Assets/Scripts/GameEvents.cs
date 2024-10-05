using System.Runtime.CompilerServices;
using UnityEngine;

public struct GameOverSignal
{
}

public struct ScoreChangedSignal
{
    public int Score;
}

public struct RestartPressedSignal
{

}

public struct SwipeSignal
{
    public Vector2 Direction;
}

public struct CubeCountChangedSignal
{
    public int Count;
}