using InputWrapper;
using Messaging;
using Storage;
using UnityEngine;

namespace Level
{
	public class LevelManager : MonoBehaviour
	{
		[SerializeField] private Transform gameParent;
		[SerializeField] private InputBlocker inputBlocker;
		private LevelFactory _levelFactory;

		private void Start()
		{
			var levelConfig = Resources.Load<LevelConfig>(PlayerPrefsDatabase.Instance.Load<LevelEntity>("LastSelectedLevelConfigName").LevelConfigName);
			_levelFactory = new LevelFactory();
			_levelFactory.Load(
				gameParent,
				levelConfig,
				SimpleMessenger.Instance,
				inputBlocker);
		}

		private void OnDestroy()
		{
			_levelFactory.Destroy();
		}
	}
}