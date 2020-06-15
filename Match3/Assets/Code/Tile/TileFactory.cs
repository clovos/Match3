using Level;
using Tile.DataTypes;
using UnityEngine;

namespace Tile
{
	public class TileFactory
	{
		public TileModel Load(TileConfig config, TileWorldPosition position, Transform parent, GameSettingsConfig gameSettingsConfig, int x, int y)
		{
			var prefab = Resources.Load<GameObject>(config.prefabName);
			var instance = Object.Instantiate(prefab, parent, true);

			var model = new TileModel(config.type, TileState.Normal, position);
			
			var view = instance.GetComponent<TileView>();
			view.Initialize(config, gameSettingsConfig, position);
			view.name = $"{x},{y}";
			var tileController = new TileController(model, view);
			return model;
		}
	}
}