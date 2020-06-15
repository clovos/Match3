using System;

namespace Messaging
{
	public struct SubscriptionHandle
	{
		public Guid Id { get; }
		public Type Type { get; }
		
		public SubscriptionHandle(Guid id, Type type)
		{
			Id = id;
			Type = type;
		}
	}
}