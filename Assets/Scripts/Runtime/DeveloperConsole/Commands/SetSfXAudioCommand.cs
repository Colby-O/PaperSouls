using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.MonoSystems.Audio;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "SetSfXAudioCommand", menuName = "Console Commands/Settings/SetSfXAudio")]
    internal sealed class SetSfXAudioCommand : ConsoleCommand
    {
        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            if (args.Length == 0)
            {
                msg = new($"Need to specify an audio level ex. '{Command} 0.5'.", ResponseType.Warning);
                return false;
            }

            if (!float.TryParse(args[0], out float level))
            {
                msg = new($"'{args[0]}' is not a vaild audio level.", ResponseType.Error);
                return false;
            }

            level = Mathf.Clamp01(level);

            GameManager.GetMonoSystem<IAudioMonoSystem>().SetSfXVolume(level);

            msg = new(ResponseType.None);
            return true;
        }
    }
}
