using System;
using UnityEngine;

namespace InputWrapper
{
	public class InputBlocker : MonoBehaviour
	{
		private float _blockedUntil;
		
		public bool IsBlocking()
		{
			return _blockedUntil > Time.time;
		}

		public void AddBlockTime(float timeAmount)
		{
			_blockedUntil = Mathf.Max(Time.time + timeAmount, _blockedUntil + timeAmount);
		}
	}
}