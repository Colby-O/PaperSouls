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
        private readonly List<Action<TMessage>> listeners = new();
        public int ListenerCount => listeners.Count;

        public void AddListener(Action<TMessage> listener)
        {
            listeners.Add(listener);
        }

        public void RemoveListener(Action<TMessage> listener)
        {
            listeners.Remove(listener);
        }

        public void Emit(TMessage msg)
        {
            foreach (var listener in listeners)
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
