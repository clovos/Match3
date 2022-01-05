using Tile.DataTypes;
using UnityEngine;
using Zenject;

namespace InputWrapper
{
	public class InputEventReporter : MonoBehaviour
	{
		public delegate void SwipeEvent(TileWorldPosition position, float angle);
		public event SwipeEvent OnSwipe;

		[SerializeField] private float swipeGestureMinimumDistance;
		
		private Camera _camera;
		private Vector2 _touchStartedPosition;
		private Vector2 _touchEndedPosition;

		[Inject]
		private IInputBlocker _inputBlocker;

		private void Start()
		{
			_camera = Camera.main;
		}

		private void Update()
		{
			if (_inputBlocker.IsBlocking()) return;
			
			if (Input.GetMouseButtonDown(0))
			{
				_touchStartedPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
			}
			
			if (Input.GetMouseButtonUp(0))
			{
				_touchEndedPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
				
				if(Vector2.Distance(_touchStartedPosition, _touchEndedPosition) > swipeGestureMinimumDistance)
					OnSwipe?.Invoke(new TileWorldPosition{X = _touchStartedPosition.x, Y = _touchStartedPosition.y}, CalculateAngle());
			}
		}

		private float CalculateAngle()
		{
			return Mathf.Atan2(_touchEndedPosition.y - _touchStartedPosition.y, 
				_touchEndedPosition.x - _touchStartedPosition.x) * 180 / Mathf.PI;
			
		}
	}
}