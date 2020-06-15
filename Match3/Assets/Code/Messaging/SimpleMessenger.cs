using System;
using System.Collections.Generic;

namespace Messaging
{
	public class SimpleMessenger : IMessenger
	{
		public static SimpleMessenger Instance => _instance ?? (_instance = new SimpleMessenger());
		private static SimpleMessenger _instance;

		private readonly Dictionary<Type, List<ICallbackHandler>> _subscriberList = new Dictionary<Type, List<ICallbackHandler>>();
		
		public SubscriptionHandle Subscribe<T>(Action<T> callback) where T : Message
		{
			var handleId = Guid.NewGuid();
			var type = typeof(T);
			var subData = new CallbackHandler<T>(handleId, callback);
			if(!_subscriberList.ContainsKey(type)) 
				_subscriberList[type] = new List<ICallbackHandler> { subData };
			else
				_subscriberList[type].Add(subData);
			
			return new SubscriptionHandle(handleId, type);
		}

		public void Unsubscribe<T>(SubscriptionHandle handle) where T : Message
		{
			_subscriberList[handle.Type].RemoveAll(s => s.Id == handle.Id);
		}

		public void Publish<T>(T message) where T : Message
		{
			if (!_subscriberList.ContainsKey(typeof(T))) return;
			_subscriberList[typeof(T)].ForEach(s => s.Execute(message));
		}

		public void Clear()
		{
			_subscriberList.Clear();
		}
	}
	
	public class CallbackHandler<T> : ICallbackHandler
	{
		public Guid Id { get; }
		public Action<T> Callback { get; }

		public CallbackHandler(Guid id, Action<T> callback)
		{
			Id = id;
			Callback = callback;
		}

		public Guid GetId()
		{
			return Id;
		}
		
		public void Execute(object data)
		{
			var typedData = (T)data;
			Callback(typedData);
		}
	}
	
	public interface ICallbackHandler
	{
		void Execute(object data);
		Guid Id { get; }
	}
}