using Tile.DataTypes;

namespace Tile
{
	public class TileModel
	{
		public delegate void PositionEvent(TileWorldPosition position, int step);
		public event PositionEvent OnPositionChanged;

		public delegate void StateEvent(TileState state, int step);
		public event StateEvent OnStateChanged;

		public TileType Type => _type;
		public TileState State => _state;

		private TileWorldPosition _position;
		private TileState _state;
		private readonly TileType _type;

		public void SetState(TileState state, int step) 
		{
			_state = state;
			OnStateChanged?.Invoke(state, step);
		}
		
		public void SetPosition(TileWorldPosition position, int step) 
		{
			_position = position;
			OnPositionChanged?.Invoke(position, step);
		}

		public TileModel(TileType type, TileState state, TileWorldPosition position)
		{
			_position = position;
			_state = state;
			_type = type;
		}
	}
}