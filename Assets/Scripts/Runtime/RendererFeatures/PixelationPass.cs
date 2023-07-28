using PaperSouls.Runtime.RendererFeatures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PaperSouls.Runtime.RendererFeatures
{
    internal sealed class PixelationPass : ScriptableRenderPass
    {
        // Variable must be called settings
        private PixelationFeatureSettings settings;

        private RenderTargetIdentifier _cameraColorTargetIdent;
        private RenderTargetIdentifier _pixelTargetIdent;

        private int _pixelTargetID = Shader.PropertyToID("_PixelTarget");

        private Material _materialToBlit;
        private int _pixelScreenWidth;
        private int _pixelScreenHeight;

        public PixelationPass(PixelationFeatureSettings settings)
        {
            this.settings = settings;
            this.renderPassEvent = settings.PassEvent;
            if (this._materialToBlit == null) this._materialToBlit = CoreUtils.CreateEngineMaterial("Hidden/Pixelize");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            _cameraColorTargetIdent = renderingData.cameraData.renderer.cameraColorTargetHandle;
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

            _pixelScreenHeight = settings.ScreenHeight;
            _pixelScreenWidth = (int)(_pixelScreenHeight * renderingData.cameraData.camera.aspect + 0.5f);

            _materialToBlit.SetVector("_PixelCount", new Vector2(_pixelScreenWidth, _pixelScreenHeight));
            _materialToBlit.SetVector("_PixelSize", new Vector2(1.0f / _pixelScreenWidth, 1.0f / _pixelScreenHeight));
            _materialToBlit.SetVector("_HalfPixelSize", new Vector2(0.5f / _pixelScreenWidth, 0.5f / _pixelScreenHeight));

            descriptor.height = _pixelScreenHeight;
            descriptor.width = _pixelScreenWidth;

            cmd.GetTemporaryRT(_pixelTargetID, descriptor, FilterMode.Point);
            _pixelTargetIdent = new RenderTargetIdentifier(_pixelTargetID);

        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (cmd == null) Debug.LogError("CMD is null!");
            cmd.ReleaseTemporaryRT(_pixelTargetID);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Fetches a comand buffer
            CommandBuffer cmd = CommandBufferPool.Get();
            cmd.Clear();

            using (new ProfilingScope(cmd, new ProfilingSampler("Pixelation Pass")))
            {
                cmd.Blit(_cameraColorTargetIdent, _pixelTargetIdent, _materialToBlit);
                cmd.Blit(_pixelTargetIdent, _cameraColorTargetIdent);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Release();
        }
    }
}
