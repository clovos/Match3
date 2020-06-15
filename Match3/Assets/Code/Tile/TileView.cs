using System.Collections;
using Level;
using Tile.DataTypes;
using UnityEngine;

namespace Tile
{
	public class TileView : MonoBehaviour
	{
		private GameObject _model;
		private GameSettingsConfig _gameSettingsConfig;
		private GameObject _matchEffectObject;

		public void Initialize(TileConfig config, GameSettingsConfig gameSettingsConfig, TileWorldPosition position)
		{
			_gameSettingsConfig = gameSettingsConfig;
			_model = Instantiate(Resources.Load<GameObject>("TileModels/" + config.modelName), transform, false);
			_matchEffectObject = Instantiate(Resources.Load<GameObject>("TileEffects/" + config.matchEffectName), transform, false);
			_matchEffectObject.SetActive(false);
			SetPositionInstant(position);
		}
		
		public void SetPosition(TileWorldPosition position, int step)
		{
			StartCoroutine(LerpPositionOnStep(position, step));
		}
		
		public void SetState(TileState state, int step)
		{
			if(state == TileState.Match) StartCoroutine(ExplodeOnStep(step));
			else if( state == TileState.Replace) StartCoroutine(RemoveOnStep(step));
		}

		private IEnumerator LerpPositionOnStep(TileWorldPosition position, int step)
		{
			yield return new WaitForSeconds(_gameSettingsConfig.stepTime * step);
			
			var targetPosition = new Vector3(position.X, position.Y, transform.position.z);
			var timePassed = 0f;
			var startPosition = transform.position;
			while (timePassed < _gameSettingsConfig.stepTime)
			{
				timePassed += Time.deltaTime;
				transform.position = Vector3.Lerp(startPosition, targetPosition, timePassed/_gameSettingsConfig.stepTime);
				yield return null;
			}
		}

		private IEnumerator ExplodeOnStep(int step)
		{
			yield return new WaitForSeconds(_gameSettingsConfig.stepTime * step);
			_matchEffectObject.SetActive(true);
			_model.SetActive(false);
			yield return new WaitForSeconds(_gameSettingsConfig.explostionAnimationTime);
			Destroy(gameObject);
		}
		
		private IEnumerator RemoveOnStep(int step)
		{
			yield return new WaitForSeconds(_gameSettingsConfig.stepTime * step);
			Destroy(gameObject);
		}
		
		private void SetPositionInstant(TileWorldPosition position)
		{
			transform.position = new Vector3(position.X, position.Y, transform.position.z);
		}
	}
}