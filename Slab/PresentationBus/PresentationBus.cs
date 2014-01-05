using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Slab.PresentationBus
{
    public class PresentationBus : IPresentationBus
    {
        private readonly SynchronizationContext _uicontext;
        private readonly Dictionary<Type, Subscribers> _subscribersByEventType;
 
        public PresentationBus()
        {
            _uicontext = SynchronizationContext.Current;
            _subscribersByEventType = new Dictionary<Type, Subscribers>();
        }

        public void Subscribe<T>(IHandlePresentationEvent<T> handler) where T : IPresentationEvent
        {
            Subscribe(typeof (T), handler);
        }
        public void Subscribe<T>(IHandlePresentationEventAsync<T> handler) where T : IPresentationEvent
        {
            Subscribe(typeof (T), handler);
        }

        public void Subscribe(IHandlePresentationEvents instance)
        {
            ForEachHandledEvent(instance, x => Subscribe(x, instance));
        }

        private void Subscribe(Type eventType, object instance)
        {
            Subscribers subscribersForEventType;

            if (_subscribersByEventType.ContainsKey(eventType))
            {
                subscribersForEventType = _subscribersByEventType[eventType];
            }
            else
            {
                subscribersForEventType = new Subscribers();
                _subscribersByEventType.Add(eventType, subscribersForEventType);
            }

            subscribersForEventType.AddSubscriber(instance);
        }

        public void UnSubscribe<T>(IHandlePresentationEvent<T> handler) where T : IPresentationEvent
        {
            UnSubscribe(typeof (T), handler);
        }

        public void UnSubscribe(IHandlePresentationEvents instance)
        {
            ForEachHandledEvent(instance, x => UnSubscribe(x, instance));
        }

        private void UnSubscribe(Type eventType, object handler)
        {
            if (_subscribersByEventType.ContainsKey(eventType))
            {
                _subscribersByEventType[eventType].RemoveSubscriber(handler);
            }
        }

        public async Task PublishAsync<T>(T presentationEvent) where T : IPresentationEvent
        {
            var type = presentationEvent.GetType();
            var typeInfo = type.GetTypeInfo();
            foreach (var subscribedType in _subscribersByEventType.Keys.Where(t => t.GetTypeInfo().IsAssignableFrom(typeInfo)).ToArray())
            {
                await _subscribersByEventType[subscribedType].PublishEvent(presentationEvent);
            }
        }


        private void ForEachHandledEvent(IHandlePresentationEvents instance, Action<Type> callback)
        {
            var handlesEventsType = typeof(IHandlePresentationEvents);

            var type = instance.GetType();

            var interfaceTypes = type.GetTypeInfo().ImplementedInterfaces;
            foreach (var interfaceType in interfaceTypes.Where(x => x.IsConstructedGenericType && handlesEventsType.GetTypeInfo().IsAssignableFrom(x.GetTypeInfo())))
            {
                var eventType = interfaceType.GenericTypeArguments.First();
                callback(eventType);
            }
        }

        protected void DispatchCall(SendOrPostCallback call)
        {
            if (SynchronizationContext.Current != _uicontext)
            {
                _uicontext.Post(call, null);
            }
            else
            {
                call(null);
            }
        }

        internal class Subscribers
        {
            private readonly List<WeakReference> _subscribers; 

            public Subscribers()
            {
                _subscribers = new List<WeakReference>();
            }

            public void AddSubscriber<T>(IHandlePresentationEvent<T> instance) where T : IPresentationEvent
            {
                AddSubscriber((object)instance);
            }
            public void AddSubscriber<T>(IHandlePresentationEventAsync<T> instance) where T : IPresentationEvent
            {
                AddSubscriber((object)instance);
            }
            public void AddSubscriber(object instance)
            {
                if (_subscribers.Any(s => s.Target == instance))
                    return;

                _subscribers.Add(new WeakReference(instance));
            }

            public void RemoveSubscriber<T>(IHandlePresentationEvent<T> instance) where T : IPresentationEvent
            {
                RemoveSubscriber((object)instance);
            }
            public void RemoveSubscriber(object instance)
            {
                var subscriber = _subscribers.SingleOrDefault(s => s.Target == instance);
                if (subscriber != null)
                {
                    _subscribers.Remove(subscriber);
                }
            }

            public async Task<bool> PublishEvent<T>(T presentationEvent) where T : IPresentationEvent
            {
                var anySubscribersStillListening = false;
                var presentationRequest = presentationEvent as IPresentationRequest;

                foreach (var subscriber in _subscribers.Where(s => s.Target != null))
                {
                    if (presentationRequest == null || presentationRequest.IsHandled == false)
                    {
                        var syncHandler = subscriber.Target as IHandlePresentationEvent<T>;
                        if (syncHandler != null)
                            syncHandler.Handle(presentationEvent);
                        
                        var asyncHandler = subscriber.Target as IHandlePresentationEventAsync<T>;
                        if (asyncHandler != null)
                            await asyncHandler.HandleAsync(presentationEvent);
                    }
                    anySubscribersStillListening = true;
                }

                if (presentationEvent.MustBeHandled && presentationEvent.IsHandled == false)
                {
                    throw new InvalidOperationException(string.Format("Event {0} was not handled.", presentationEvent.GetType().Name));
                }
                return anySubscribersStillListening;
            }
        }
    }
}