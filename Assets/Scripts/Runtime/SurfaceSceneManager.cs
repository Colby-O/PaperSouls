using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.MonoSystems.Audio;
using PaperSouls.Core;

namespace PaperSouls
{
    internal sealed class SurfaceSceneManager : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Emit<PlayAudioMessage>(new(0, Runtime.MonoSystems.Audio.AudioType.Music));
        }
    }
}
