using System;
using UnityEngine;
using Zenject;

public class SwipeDetection : Zenject.IInitializable, IDisposable, IFixedTickable
{
	private SwipeControl _swipeControl;
	private Vector2 initialPos;
	private bool _isInSwipe = false;
	private Vector2 currentPos => _swipeControl.SwipeDetection.Position.ReadValue<Vector2>();
	private SignalBus _signalBus;
	private Settings _settings;

    private SwipeDetection(SwipeControl swipeControl, Settings settings, SignalBus signalBus)
    {
		_swipeControl = swipeControl;
        _settings = settings;
		_signalBus = signalBus;
    }

	public void Initialize()
    {
		_swipeControl.Enable();
		_swipeControl.SwipeDetection.Press.performed += ctx => OnSwipeBegin();
		_swipeControl.SwipeDetection.Press.canceled += ctx => OnSwipeEnd();
    }

    public void Dispose()
    {
        _swipeControl.Disable();
		_swipeControl.SwipeDetection.Press.performed -= ctx => OnSwipeBegin();
		_swipeControl.SwipeDetection.Press.canceled -= ctx => OnSwipeEnd();
    }

    public void FixedTick()
    {
        if(_isInSwipe)
		{
			DetectSwipe();
		}
    }

	private void DetectSwipe () 
	{
		Vector2 delta = currentPos - initialPos;
		Vector2 direction = Vector2.zero;

		if(Mathf.Abs(delta.x) > _settings.SwipeResistance)
		{
			direction.x = Mathf.Clamp(delta.x, -1, 1);
		}
		if(Mathf.Abs(delta.y) > _settings.SwipeResistance)
		{
			direction.y = Mathf.Clamp(delta.y, -1, 1);
		}

		if(direction != Vector2.zero)
        {
			_signalBus.Fire(new SwipeSignal() { Direction = direction });
			_isInSwipe = false;
        }
	}

	private void OnSwipeBegin()
	{
		initialPos = currentPos;
		_isInSwipe = true;
	}

	private void OnSwipeEnd()
	{
		_isInSwipe = false;
	}

    [Serializable]
	public class Settings
	{
		public float SwipeResistance;
	}
}