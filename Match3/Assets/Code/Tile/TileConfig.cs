using UnityEngine;

namespace Tile
{
	[CreateAssetMenu(fileName = "Tile", menuName = "Match/Tile")]
	public class TileConfig : ScriptableObject
	{
		public string prefabName = "Tile";
		public string modelName;
		public TileType type;
		public string matchEffectName;
	}
}