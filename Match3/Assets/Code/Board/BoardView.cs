using System.Collections;
using Level;
using TMPro;
using UnityEngine;

namespace Board
{
	public class BoardView : MonoBehaviour
	{
		[SerializeField] private Transform backgroundContainer;

		public void Initialize(BoardConfig config)
		{
			backgroundContainer.localScale = new Vector3(0.55f * config.boardWidth, 0.55f * config.boardHeight);
		}
	}
}