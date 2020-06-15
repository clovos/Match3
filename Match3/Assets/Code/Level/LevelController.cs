using InputWrapper;
using Messaging;
using Messaging.Messages;
using Storage;
using UnityEngine.SceneManagement;

namespace Level
{
	public class LevelController
	{
		private readonly LevelModel _model;
		private readonly LevelView _view;
		
		private readonly IMessenger _messenger;
		private readonly SubscriptionHandle _updateScoreSubscriptionHandle;
		private readonly SubscriptionHandle _updateMoveSubscriptionHandle;
		private LevelConfig _config;
		private InputBlocker _inputBlocker;

		public LevelController(
			LevelModel model, 
			LevelView view,
			LevelConfig config,
			IMessenger messenger,
			InputBlocker inputBlocker)
		{
			_model = model;
			_view = view;
			_messenger = messenger;
			_config = config;
			_inputBlocker = inputBlocker;

			_model.MovesLeft = config.moves;
			
			_updateScoreSubscriptionHandle = _messenger.Subscribe<ScoreUpdateMessage>(UpdateScore);
			_updateMoveSubscriptionHandle = _messenger.Subscribe<MoveUpdateMessage>(UpdateMove);
			_updateMoveSubscriptionHandle = _messenger.Subscribe<BoardIdleMessage>(UpdateBoardIdle);
			
			_view.OnOutOfMoves += OutOfMoves;
			_view.OnReachedScoreGoal += ReachedScoreGoal;
			_view.OnBlockInput += BlockInput;
			_view.UpdateScore(_model.Score, 0);
			_view.UpdateMovesLeft(_model.MovesLeft);
		}
		
		private void UpdateScore(ScoreUpdateMessage message)
		{
			_model.Score += message.Score;
			_view.UpdateScore(_model.Score, message.Step);
		}

		private void UpdateMove(MoveUpdateMessage message)
		{
			_model.MovesLeft--;
			_view.UpdateMovesLeft(_model.MovesLeft);
		}
		
		private void UpdateBoardIdle(BoardIdleMessage message)
		{
			_view.UpdateBoardIdle(_model.MovesLeft, message.Step);
		}

		private void OutOfMoves()
		{
			SceneManager.LoadScene("Menu");
		}

		private void ReachedScoreGoal()
		{
			PlayerPrefsDatabase.Instance.Save(_config.levelName, new LevelProgressEntity{Completed = true, UsedMoves = _config.moves -_model.MovesLeft});
			SceneManager.LoadScene("Menu");
		}

		private void BlockInput(float length)
		{
			_inputBlocker.AddBlockTime(length);
		}
		
		public void Destroy()
		{
			_messenger.Unsubscribe<ScoreUpdateMessage>(_updateScoreSubscriptionHandle);
			_messenger.Unsubscribe<ScoreUpdateMessage>(_updateMoveSubscriptionHandle);
			_messenger.Unsubscribe<BoardIdleMessage>(_updateMoveSubscriptionHandle);
			_view.OnOutOfMoves -= OutOfMoves;
		}
	}
}