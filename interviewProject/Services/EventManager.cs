using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace interviewProject.Events
{
    public static class EventManager
    {
        private static readonly Dictionary<string, Delegate> Events = new Dictionary<string, Delegate>();

        public static void Register<T>(string eventName, Action<T> handler)
        {
            Console.WriteLine($"Registering {eventName} event.");

            if (Events.ContainsKey(eventName))
            {
                // Ensure that the handler being combined has the correct type
                if (Events[eventName] is Action<T> existingHandler)
                {
                    Events[eventName] = existingHandler + handler;
                }
                else
                {
                    throw new InvalidOperationException($"Event {eventName} is already registered with a different type.");
                }
            }
            else
            {
                Events[eventName] = handler;
            }
        }

        public static void Unregister<T>(string eventName, Action<T> handler)
        {
            if (Events.ContainsKey(eventName))
            {
                if (Events[eventName] is Action<T> existingHandler)
                {
                    Events[eventName] = existingHandler - handler;

                    if (Events[eventName] == null)
                    {
                        Events.Remove(eventName);
                    }
                }
            }
        }

        public static void Emit<T>(string eventName, T eventData)
        {
            if (Events.ContainsKey(eventName))
            {
                if (Events[eventName] is Action<T> handler)
                {
                    handler?.Invoke(eventData);
                }
                else
                {
                    Console.WriteLine($"No handler of type {typeof(T)} found for event {eventName}");
                }
            }
        }

        public static void RegisterEventReceivers(IServiceCollection services, Assembly assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();
            var eventReceiverTypes = assembly.GetTypes()
                .Where(t => typeof(IEventReceiver).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            foreach (var receiverType in eventReceiverTypes)
            {
                services.AddSingleton(typeof(IEventReceiver), receiverType);
                Console.WriteLine($"Registered {receiverType.Name} as event receiver.");
            }
        }

        public static void InitializeEventReceivers(IServiceProvider serviceProvider)
        {
            var eventReceivers = serviceProvider.GetServices<IEventReceiver>();
            foreach (var receiver in eventReceivers)
            {
                Console.WriteLine($"Initializing {receiver.GetType().Name}");
                receiver.Register();
            }
        }
    }
}
