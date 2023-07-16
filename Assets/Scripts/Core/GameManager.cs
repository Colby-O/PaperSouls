using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Core
{
    internal abstract class GameManager : MonoBehaviour
    {
        private const string PrefabPath = "GameManager";
        protected static GameManager _instance;

        private readonly MessageManager _messageManager = new();
        private readonly MonoSystemManager _monoSystemManager = new();

        /// <summary>
        /// Adds a listener for TMessage to the scene.
        /// </summary>
        public static void AddListener<TMessage>(Action<TMessage> listener) where TMessage : IMessage => _instance._messageManager.AddListener(listener);

        /// <summary>
        /// Removes a listener for TMessage to the scene.
        /// </summary>
        public static void RemoveListener<TMessage>(Action<TMessage> listener) where TMessage : IMessage => _instance._messageManager.RemoveListener(listener);

        /// <summary>
        /// Emits an message to the GameManager.
        /// </summary>
        public static void Emit<TMessage>(TMessage msg) where TMessage : IMessage => _instance._messageManager.Emit(msg);

        /// <summary>
        /// Add a MonoSystems to the GameManager. A MonoSystem takes the place of other singleton classes.
        /// </summary>
        public static void AddMonoSystem<TMonoSystem, TBindTo>(TMonoSystem monoSystem) where TMonoSystem : IMonoSystem, TBindTo => _instance._monoSystemManager.AddMonoSystem<TMonoSystem, TBindTo>(monoSystem);

        /// <summary>
        /// Fetches an attached MonoSystem of type TMonoSystem.
        /// </summary>
        public static TMonoSystem GetMonoSystem<TMonoSystem>() => _instance._monoSystemManager.GetMonoSystem<TMonoSystem>();


        /// <summary>
        /// Initialzes the GameManager automatically on scene load.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_instance) return;

            GameManager gameManagerPreafab = Resources.Load<GameManager>(PrefabPath);
            GameManager gameManager = Instantiate(gameManagerPreafab);

            gameManager.name = gameManager.GetApplicationName();

            DontDestroyOnLoad(gameManager);

            _instance = gameManager;

            gameManager.OnInitalized();
        }

        /// <summary>
        /// Fetches the name of the application.
        /// </summary>
        protected abstract string GetApplicationName();
        
        /// <summary>
        /// Function to be ran after the GameManager is Initalized.
        /// </summary>
        protected abstract void OnInitalized();
    }
}
