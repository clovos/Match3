namespace Level
{
	[UnityEngine.CreateAssetMenu(fileName = "GameSettings", menuName = "Match/GameSettings")]
	public class GameSettingsConfig : UnityEngine.ScriptableObject
	{
		public float stepTime;
		public int matchScore;
		public float explostionAnimationTime;
	}
}