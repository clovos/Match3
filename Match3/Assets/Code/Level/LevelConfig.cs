using System;
using Board;
using UnityEngine;

namespace Level
{
	[Serializable]
	[CreateAssetMenu(fileName = "Level", menuName = "Match/Level")]
	public class LevelConfig : ScriptableObject
	{
		public string prefabName = "Level";
		public new string levelName;
		public int moves;
		public int scoreToAchive;
		public Sprite buttonArt;
		public BoardConfig board;
		public GameSettingsConfig gameSettings;
	}
}