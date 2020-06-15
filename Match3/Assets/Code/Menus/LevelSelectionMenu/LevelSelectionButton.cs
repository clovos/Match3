using System;
using Level;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Menus.LevelSelectionMenu
{
	public class LevelSelectionButton : MonoBehaviour
	{
		[SerializeField] private Image[] artImages;
		[SerializeField] private TextMeshProUGUI levelNameLabel;
		[SerializeField] private GameObject completedObject;
		[SerializeField] private GameObject lockedObject;
		
		public void Initialize(LevelConfig levelConfig, bool completed, bool locked, UnityAction buttonPressedCallback)
		{
			if(!locked) GetComponent<Button>().onClick.AddListener(buttonPressedCallback);
			
			foreach (var image in artImages)
			{
				image.sprite = levelConfig.buttonArt;
			}

			levelNameLabel.text = levelConfig.levelName;
			completedObject.SetActive(completed);
			lockedObject.SetActive(locked);
		}
	}
}