using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;

namespace PaperSouls.Runtime.MonoSystems.Audio
{
    internal interface IAudioMonoSystem : IMonoSystem
    {
        /// <summary>
        /// Sets the overall volume level
        /// </summary>
        public void SetOverallVolume(float volume);

        /// <summary>
        /// Sets the SFX volume level
        /// </summary>
        public void SetSfXVolume(float volume);

        /// <summary>
        /// Sets the music volume level
        /// </summary>
        public void SetMusicVolume(float volume);

        /// <summary>
        /// Gets the overall volume level
        /// </summary>
        public float GetOverallVolume();

        /// <summary>
        /// Gets the SFX volume level
        /// </summary>
        public float GetSfXVolume();

        /// <summary>
        /// Gets the music volume level
        /// </summary>
        public float GetMusicVolume();
    }
}
