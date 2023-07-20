using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;

namespace PaperSouls.Runtime.MonoSystems.DataPersistence
{
    internal sealed class SetProfileIDMessage : IMessage
    {
        public int ProfileID;

        public SetProfileIDMessage(int profileID)
        {
            ProfileID = profileID;
        }
    }

    internal sealed class ChangeProfileMessage : IMessage
    {
        public string ProfileName;

        public ChangeProfileMessage(string profileName)
        {
            ProfileName = profileName;
        }
    }

    internal sealed class DeleteProfileMessage : IMessage
    {
        public string ProfileName;

        public DeleteProfileMessage(string profileName)
        {
            ProfileName = profileName;
        }
    }
}
