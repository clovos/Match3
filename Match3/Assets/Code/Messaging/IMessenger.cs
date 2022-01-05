using System;

namespace Messaging
{
	public interface IMessenger
	{
		SubscriptionHandle Subscribe<T>(Action<T> callback) where T : IMessage;
		void Unsubscribe<T>(SubscriptionHandle handle) where T : IMessage;
		void Publish<T>(T message) where T : IMessage;
	}
}