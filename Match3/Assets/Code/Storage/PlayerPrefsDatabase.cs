using System;
using UnityEngine;

namespace Storage
{
	public class PlayerPrefsDatabase : IDatabase
	{
		public static PlayerPrefsDatabase Instance => _instance ?? (_instance = new PlayerPrefsDatabase());
		private static PlayerPrefsDatabase _instance = null;
	
		public void Save<T>(string key, T data)
		{
			var serializedObjectData = JsonUtility.ToJson(data);
			PlayerPrefs.SetString(key, serializedObjectData);
		}

		public T Load<T>(string key) where T : new()
		{
			var serializedObjectData = PlayerPrefs.GetString(key, string.Empty);
			return serializedObjectData == string.Empty ? new T() : JsonUtility.FromJson<T>(serializedObjectData);
		}
	}
}