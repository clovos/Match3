using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Level
{
	public class LevelView : MonoBehaviour
	{
		public delegate void BlockInput(float time);
		public event BlockInput OnBlockInput;
		
		public delegate void OutOfMoves();
		public event OutOfMoves OnOutOfMoves;
		
		public delegate void ReachedScoreGoal();
		public event ReachedScoreGoal OnReachedScoreGoal;
		
		[SerializeField] private TextMeshProUGUI scoreLabel;
		[SerializeField] private TextMeshProUGUI movesLeftLabel;
		[SerializeField] private Button backButton;
		[SerializeField] private TextMeshProUGUI flashTextLabel;
		[SerializeField] private Animator flashTextAnimator;
		[SerializeField] private float flashTextAnimationLength;
		private LevelConfig _config;
		private GameSettingsConfig _gameSettingsConfig;
		private static readonly int FlashText = Animator.StringToHash("FlashText");

		public void Initialize(LevelConfig config)
		{
			_config = config;
			_gameSettingsConfig = config.gameSettings;
			
			backButton.onClick.AddListener(OnBackPressed);
		}

		private void OnBackPressed()
		{
			OnOutOfMoves?.Invoke();
		}

		public void UpdateScore(int score, int steps)
		{
			StartCoroutine(UpdateScoreOnStep(score, steps));
		}

		private IEnumerator UpdateScoreOnStep(int score, int steps)
		{
			yield return new WaitForSeconds(steps * _gameSettingsConfig.stepTime);
			scoreLabel.text = $"Score : {score}/{_config.scoreToAchive}";
			if (score >= _config.scoreToAchive)
			{
				flashTextLabel.text = "Level completed!";
				flashTextAnimator.SetTrigger(FlashText);
				OnBlockInput?.Invoke(flashTextAnimationLength);
				yield return new WaitForSeconds(flashTextAnimationLength);
				OnReachedScoreGoal?.Invoke();
			}
		}

		public void UpdateMovesLeft(int movesLeft)
		{
			movesLeftLabel.text = $"Moves left : {movesLeft}";
		}
		
		public void UpdateBoardIdle(int movesLeft, int steps)
		{
			StartCoroutine(UpdateBoardIdleOnStep(movesLeft, steps));
		}
		
		private IEnumerator UpdateBoardIdleOnStep(int movesLeft, int steps)
		{
			yield return new WaitForSeconds(steps * _gameSettingsConfig.stepTime);
			if (movesLeft == 0)
			{
				flashTextLabel.text = "You lost!";
				flashTextAnimator.SetTrigger(FlashText);
				OnBlockInput?.Invoke(flashTextAnimationLength);
				yield return new WaitForSeconds(flashTextAnimationLength);
				OnOutOfMoves?.Invoke();
			}
		}
	}
}