using Tile.DataTypes;

namespace Tile
{
	public class TileController
	{
		private readonly TileModel _model;
		private readonly TileView _view;
		
		public TileController(TileModel model, TileView view)
		{
			_model = model;
			_view = view;
			_model.OnPositionChanged += OnPositionChanged;
			_model.OnStateChanged += OnStateChanged;
		}
		
		private void OnPositionChanged(TileWorldPosition position, int steps)
		{
			_view.SetPosition(position, steps);
		}
		
		private void OnStateChanged(TileState state, int steps)
		{
			_view.SetState(state, steps);
		}
	}
}