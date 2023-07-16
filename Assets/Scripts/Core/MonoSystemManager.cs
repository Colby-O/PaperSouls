using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Core
{
    internal sealed class MonoSystemManager 
    {
        private readonly Dictionary<Type, IMonoSystem> _monoSystems = new();

        /// <summary>
        /// Adds a MonoSystem to the master list.
        /// </summary>
        public void AddMonoSystem<TMonoSystem, TBindTo>(TMonoSystem monoSystem) where TMonoSystem : TBindTo, IMonoSystem
        {
            if (monoSystem == null) throw new Exception($"{nameof(monoSystem)} cannot be null!!!");
            Type monoSystemType = typeof(TBindTo);
            _monoSystems[monoSystemType] = monoSystem;
        }

        /// <summary>
        /// Removes a MonoSystem from the master list.
        /// </summary>
        public TMonoSystem GetMonoSystem<TMonoSystem>()
        {
            Type monoSystemType = typeof(TMonoSystem);

            if (_monoSystems.TryGetValue(monoSystemType, out var monoSystem)) return (TMonoSystem)monoSystem;
            else throw new Exception($"MonoSystem {monoSystemType} does not exist");
        }
    }
}
