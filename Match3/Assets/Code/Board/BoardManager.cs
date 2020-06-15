using System;
using Messaging;
using Level;
using InputWrapper;
using Storage;
using UnityEngine;

namespace Board
{
	public class BoardManager : MonoBehaviour
	{
		[SerializeField] private Transform boardParent;
		[SerializeField] private InputEventReporter inputEventReporter;
		[SerializeField] private InputBlocker inputBlocker;
		private BoardFactory _boardFactory;

		private void Start()
		{
			var levelConfig = Resources.Load<LevelConfig>(PlayerPrefsDatabase.Instance.Load<LevelEntity>("LastSelectedLevelConfigName").LevelConfigName);
			_boardFactory = new BoardFactory();
			_boardFactory.Load(
				levelConfig, 
				boardParent,
				inputEventReporter, 
				inputBlocker, 
				SimpleMessenger.Instance);
		}

		private void OnDestroy()
		{
			_boardFactory.Destroy();
		}
	}
}