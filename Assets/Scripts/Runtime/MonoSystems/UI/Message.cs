using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.UI.View;

namespace PaperSouls.Runtime.MonoSystems.UI
{
    internal sealed class ChangeViewMessage : IMessage
    {
        public View NextView { get; private set; }
        public bool Remember { get; private set; }

        public ChangeViewMessage(View nextView, bool remember = true)
        {
            NextView = nextView;
            Remember = remember;
        }
    }

    internal sealed class ResetViewMessage : IMessage
    {
        public ResetViewMessage() { }
    }
}
