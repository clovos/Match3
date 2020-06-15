using Level;
using Storage;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menus.LevelSelectionMenu
{
	public class LevelSelectionMenu : MonoBehaviour
	{
		[SerializeField] private LevelConfig[] levelConfigs;
		[SerializeField] private GameObject levelButtonPrefab;
		[SerializeField] private Transform levelGridLayout;

		private void Start()
		{
			for (var i = 0; i < levelConfigs.Length; i++)
			{
				var button = Instantiate(levelButtonPrefab, levelGridLayout).GetComponent<LevelSelectionButton>();
				var index = i;
				button.Initialize(levelConfigs[i], 
					PlayerPrefsDatabase.Instance.Load<LevelProgressEntity>(levelConfigs[i].levelName).Completed,
					i > 0 && !PlayerPrefsDatabase.Instance.Load<LevelProgressEntity>(levelConfigs[i - 1].levelName).Completed,
					() =>
					{
						PlayerPrefsDatabase.Instance.Save("LastSelectedLevelConfigName", new LevelEntity{ LevelConfigName = levelConfigs[index].name} );
						SceneManager.LoadScene("Game");
					});
			}
		}
	}
}