using Messaging;
using Level;
using InputWrapper;
using UnityEngine;

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
			InputBlocker inputBlocker,
			IMessenger messenger)
		{
			var prefab = Resources.Load<GameObject>(config.board.prefabName);
			var instance = GameObject.Instantiate(prefab, parent, false);
			_model = new BoardModel();
			_view = instance.GetComponent<BoardView>();
			_view.Initialize(config.board);
			_controller = new BoardController(
				_model, 
				_view, 
				config.board, 
				config.gameSettings, 
				inputEventReporter, 
				inputBlocker, 
				messenger);
		}

		public void Destroy()
		{
			_controller.Destroy();
		}
	}
}