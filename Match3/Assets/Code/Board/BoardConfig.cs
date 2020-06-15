using Tile;
using UnityEngine;

namespace Board
{
    [CreateAssetMenu(fileName = "Board", menuName = "Match/Board")]
    public class BoardConfig : ScriptableObject
    {
        public string prefabName = "Board";

        public int boardWidth;
        public int boardHeight;

        public float tileWidth;
        public float tileHeight;

        public TileConfig[] tiles;
    }
}