using System;
using Messaging;
using Level;
using InputWrapper;
using Storage;
using UnityEngine;
using Zenject;

namespace Board
{
	public class BoardManager : MonoBehaviour
	{
		private const string lastSelectedLevelDatabaseKey = "LastSelectedLevelConfigName";

		[SerializeField] private Transform boardParent;
		[SerializeField] private InputEventReporter inputEventReporter;

		private BoardFactory _boardFactory;

		[Inject]
		private readonly IDatabase _database;

		[Inject]
		private readonly IInputBlocker _inputBlocker;

		[Inject]
		private readonly IMessenger _messenger;

		private void Start()
		{
			var levelConfig = Resources.Load<LevelConfig>(_database.Load<LevelEntity>(lastSelectedLevelDatabaseKey).LevelConfigName);
			_boardFactory = new BoardFactory();
			_boardFactory.Load(
				levelConfig, boardParent, inputEventReporter, 
				_messenger, _inputBlocker);
		}

		private void OnDestroy()
		{
			_boardFactory.Destroy();
		}
	}
}