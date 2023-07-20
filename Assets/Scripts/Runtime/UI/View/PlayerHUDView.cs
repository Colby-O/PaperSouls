using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.UI.View
{
    internal sealed class PlayerHUDView : View
    {
        public override void Init() { }

        public override void Show()
        {
            base.Show();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
