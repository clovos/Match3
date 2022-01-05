namespace Messaging.Messages
{
	public class ScoreUpdateMessage : IMessage
	{
		public int Score;
		public int Step;
	}
}