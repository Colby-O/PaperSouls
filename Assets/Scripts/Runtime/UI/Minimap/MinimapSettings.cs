using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.UI.Minimap
{
    internal class MinimapSettings : MonoBehaviour
    {
        public Transform TargetToFollow;
        public bool RotateWithTarget = true;
        public float CameraHeight = 10.0f;
    }
}
