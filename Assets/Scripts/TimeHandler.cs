using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TimeHandler
{
    private SwipeControl _swipeConrol;

    public TimeHandler(SwipeControl swipeConrol)
    {
        _swipeConrol = swipeConrol;
    }

    public void StartTime()
    {
        _swipeConrol.Enable();
        Time.timeScale = 1;
    }

    public void StopTime()
    {
        _swipeConrol.Disable();
        Time.timeScale = 0;
    }
}