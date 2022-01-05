namespace InputWrapper
{
	public interface IInputBlocker
	{
		bool IsBlocking();
		void AddBlockTime(float timeAmount);
	}
}
