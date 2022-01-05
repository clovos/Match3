using Messaging;
using Level;
using InputWrapper;
using UnityEngine;
using Zenject;

namespace Board
{
	public class BoardFactory
	{
		private BoardModel _model;
		private BoardView _view;
		private BoardController _controller;

		public void Load(LevelConfig config, 
			Transform parent,
			InputEventReporter inputEventReporter,
			IMessenger messenger,
			IInputBlocker inputBlocker)
		{
			var prefab = Resources.Load<GameObject>(config.board.prefabName);
			var instance = Object.Instantiate(prefab, parent, false);
			_model = new BoardModel();
			_view = instance.GetComponent<BoardView>();
			_view.Initialize(config.board);
			_controller = new BoardController(
				_model, _view, config.board, config.gameSettings, 
				inputEventReporter, messenger, inputBlocker);
		}

		public void Destroy()
		{
			_controller.Destroy();
		}
	}
}