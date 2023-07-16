using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;

namespace PaperSouls.Runtime.MonoSystems.Audio
{
    internal sealed class PlayAudioMessage : IMessage
    {
        public AudioType AudioType { get; private set; }
        public string AudioName { get; private set; }
        public int AudioID { get; private set; }

        public bool AllowOverlay { get; private set; }

        public PlayAudioMessage(string audioName, AudioType audioType, bool allowOverlay = true)
        {
            AudioType = audioType;
            AudioName = audioName;
            AudioID = -1;
            AllowOverlay = allowOverlay;
        }

        public PlayAudioMessage(int audioID, AudioType audioType, bool allowOverlay = true)
        {
            AudioType = audioType;
            AudioName = null;
            AudioID = audioID;
            AllowOverlay = allowOverlay;
        }
    }

    internal sealed class StopAudioMessage : IMessage
    {
        public AudioType AudioType { get; private set; }

        public StopAudioMessage(AudioType audioType)
        {
            AudioType = audioType;
        }
    }
}
