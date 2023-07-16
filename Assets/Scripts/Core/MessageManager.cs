using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Core
{
    internal sealed class MessageManager
    {
        private readonly Dictionary<Type, MessageListener> _listeners = new();

        public void AddListener<TMessage>(Action<TMessage> listener) where TMessage : IMessage
        {
            Type listenerType = typeof(TMessage);

            if (_listeners.TryGetValue(listenerType, out var existingListener))
            {
                MessageListener<TMessage> messageListener = existingListener as MessageListener<TMessage>;
                messageListener.AddListener(listener);
            }
            else
            {
                MessageListener<TMessage> messageListener = new();
                messageListener.AddListener(listener);

                _listeners[listenerType] = messageListener;
            }
        }

        public void RemoveListener<TMessage>(Action<TMessage> listener) where TMessage : IMessage
        {
            Type listenerType = typeof(TMessage);

            if (_listeners.TryGetValue(listenerType, out var existingListener))
            {
                MessageListener<TMessage> messageListener = existingListener as MessageListener<TMessage>;
                messageListener.RemoveListener(listener);

                if (messageListener.ListenerCount <= 0) _listeners.Remove(listenerType);
            }
        }

        public void Emit<TMessage>(TMessage msg) where TMessage : IMessage
        {
            Type listenerType = typeof(TMessage);

            if (_listeners.TryGetValue(listenerType, out var existingListener))
            {
                MessageListener<TMessage> messageListener = existingListener as MessageListener<TMessage>;
                messageListener.Emit(msg);
            }
        }
    }
}
