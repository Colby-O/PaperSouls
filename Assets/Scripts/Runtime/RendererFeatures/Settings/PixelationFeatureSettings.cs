using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PaperSouls.Runtime.RendererFeatures
{
    [System.Serializable]
    internal sealed class PixelationFeatureSettings
    {
        public bool IsEnabled = true;
        public Material MaterialToBlit;
        public RenderPassEvent PassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public int ScreenHeight = 200;
    }
}
