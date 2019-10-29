using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Notification manager which propogates application events between publishers and subscribers.
    /// </summary>
    public class NotificationManager
    {
        /// <summary>
        /// The list of subscriptions to be notified.
        /// </summary>
        private static readonly ConcurrentDictionary<string, Dictionary<ApplicationEvent, Action<object>>> _subscriptions = new ConcurrentDictionary<string, Dictionary<ApplicationEvent, Action<object>>>();

        /// <summary>
        /// Notifies the specified application event to subscribers.
        /// </summary>
        /// <param name="applicationEvent">The application event.</param>
        /// <param name="eventData">The event data.</param>
        public void Notify(ApplicationEvent applicationEvent, object eventData)
        {
            if (_subscriptions == null) return;
            foreach (var subscription in _subscriptions.Values)
            {
                Task.Run(() => subscription[applicationEvent].Invoke(eventData));
            }
        }

        /// <summary>
        /// Registers subscribers for notification.
        /// </summary>
        /// <param name="componentKey">The application key.</param>
        /// <param name="applicationEvent">The application event.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void RegisterForNotification(string componentKey, ApplicationEvent applicationEvent, Action<object> action)
        {
            var addValue = new Dictionary<ApplicationEvent, Action<object>>() { { applicationEvent, action } };
            _subscriptions.AddOrUpdate(componentKey,
                addValue, delegate
                {
                    if (_subscriptions[componentKey].ContainsKey(applicationEvent))
                    {
                        return _subscriptions[componentKey];
                    }
                    return new Dictionary<ApplicationEvent, Action<object>>() { { applicationEvent, action } };
                });
        }
    }
}