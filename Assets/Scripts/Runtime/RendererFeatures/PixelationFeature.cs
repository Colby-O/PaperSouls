using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PaperSouls.Runtime.RendererFeatures
{
    internal sealed class PixelationFeature : ScriptableRendererFeature
    {
        // Variable must be called settings
        [SerializeField] private PixelationFeatureSettings settings;

        private PixelationPass _pass;

        public override void Create()
        {
            _pass = new PixelationPass(settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (renderingData.cameraData.isSceneViewCamera) return;
#endif
            if (!settings.IsEnabled) return;
            renderer.EnqueuePass(_pass);
        }
    }
}
