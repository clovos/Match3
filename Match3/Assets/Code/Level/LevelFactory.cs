using InputWrapper;
using Messaging;
using Storage;
using UnityEngine;

namespace Level
{
	public class LevelFactory
	{
		private LevelModel _model;
		private LevelView _view;
		private LevelController _controller;

		public void Load(Transform parent, 
			LevelConfig config,
			IInputBlocker inputBlocker,
			IDatabase database,
			IMessenger messenger)
		{
			var prefab = Resources.Load<GameObject>(config.prefabName);
			var instance = Object.Instantiate(prefab, parent, false);
			_model = new LevelModel();
			_view = instance.GetComponent<LevelView>();
			_view.Initialize(config);
			_controller = new LevelController(
				_model, _view, config, inputBlocker, database, messenger);
		}

		public void Destroy()
		{
			_controller.Destroy();
		}
	}
}