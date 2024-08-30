using System;
using UnityEngine;
using Zenject;

public class SwipeDetection : IDisposable, Zenject.IInitializable
{
	private SwipeControl _swipeControl;
	private Vector2 initialPos;
	private Vector2 currentPos => _swipeControl.SwipeDetection.Position.ReadValue<Vector2>();
	private SignalBus _signalBus;
	private Settings _settings;

    private SwipeDetection(Settings settings, SignalBus signalBus)
    {
        _settings = settings;
		_signalBus = signalBus;
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
        }
	}

	public void Initialize()
    {
        _swipeControl = new SwipeControl();
		_swipeControl.Enable();
		_swipeControl.SwipeDetection.Press.performed += ctx => { initialPos = currentPos; };
		_swipeControl.SwipeDetection.Press.canceled += ctx => DetectSwipe();
    }

    public void Dispose()
    {
        _swipeControl.Disable();
		_swipeControl.SwipeDetection.Press.performed -= ctx => { initialPos = currentPos; };
		_swipeControl.SwipeDetection.Press.canceled -= ctx => DetectSwipe();
    }

    [Serializable]
	public class Settings
	{
		public float SwipeResistance;
	}
}