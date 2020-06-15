using System;

namespace Messaging
{
	public interface IMessenger
	{
		SubscriptionHandle Subscribe<T>(Action<T> callback) where T : Message;
		void Unsubscribe<T>(SubscriptionHandle handle) where T : Message;
		void Publish<T>(T message) where T : Message;
	}
}