using Level;
using Storage;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Menus.LevelSelectionMenu
{
	public class LevelSelectionMenu : MonoBehaviour
	{
		private const string lastSelectedLevelDatabaseKey = "LastSelectedLevelConfigName";

		[SerializeField] private LevelConfig[] levelConfigs;
		[SerializeField] private GameObject levelButtonPrefab;
		[SerializeField] private Transform levelGridLayout;

		[Inject]
		private IDatabase _database;

		private void Start()
		{
			for (var i = 0; i < levelConfigs.Length; i++)
			{
				var button = Instantiate(levelButtonPrefab, levelGridLayout).GetComponent<LevelSelectionButton>();
				var index = i;
				button.Initialize(levelConfigs[i], 
					_database.Load<LevelProgressEntity>(levelConfigs[i].levelName).Completed,
					i > 0 && !_database.Load<LevelProgressEntity>(levelConfigs[i - 1].levelName).Completed,
					() =>
					{
						_database.Save(lastSelectedLevelDatabaseKey, new LevelEntity{ LevelConfigName = levelConfigs[index].name} );
						SceneManager.LoadScene("Game");
					});
			}
		}
	}
}