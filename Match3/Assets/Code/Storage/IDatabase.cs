namespace Storage
{
	public interface IDatabase
	{
		void Save<T>(string key, T data);
		T Load<T>(string key) where T : new();
	}
}