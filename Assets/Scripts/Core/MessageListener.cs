using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Core
{
    internal abstract class MessageListener
    {

    }

    internal sealed class MessageListener<TMessage> : MessageListener where TMessage : IMessage
    {
        private readonly List<Action<TMessage>> _listeners = new();
        public int ListenerCount => _listeners.Count;

        /// <summary>
        /// Adds a listener to the bus
        /// </summary>
        public void AddListener(Action<TMessage> listener)
        {
            _listeners.Add(listener);
        }

        /// <summary>
        /// Removes a listener to the bus
        /// </summary>
        public void RemoveListener(Action<TMessage> listener)
        {
            _listeners.Remove(listener);
        }

        /// <summary>
        /// Emits a message.
        /// </summary>
        public void Emit(TMessage msg)
        {
            foreach (var listener in _listeners)
            {
                try
                {
                    listener.Invoke(msg);
                }
                catch (Exception e) 
                { 
                    Debug.LogException(e); 
                }
            }
        }
    }
}
