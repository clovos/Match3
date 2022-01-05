using InputWrapper;
using Messaging;
using Storage;
using UnityEngine;
using Zenject;

namespace Level
{
	public class LevelManager : MonoBehaviour
	{
		private const string lastSelectedLevelDatabaseKey = "LastSelectedLevelConfigName";

		[SerializeField] private Transform gameParent;

		private LevelFactory _levelFactory;

		[Inject]
		private readonly IInputBlocker _inputBlocker;

		[Inject]
		private readonly IMessenger _messenger;

		[Inject]
		private readonly IDatabase _database;

		private void Start()
		{
			var levelConfig = Resources.Load<LevelConfig>(_database.Load<LevelEntity>(lastSelectedLevelDatabaseKey).LevelConfigName);
			_levelFactory = new LevelFactory();
			_levelFactory.Load(gameParent, levelConfig, _inputBlocker, _database, _messenger);
		}

		private void OnDestroy()
		{
			_levelFactory.Destroy();
		}
	}
}