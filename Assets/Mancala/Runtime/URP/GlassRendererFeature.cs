using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace tana_gh.Mancala
{
    [Serializable]
    public class GlassRendererFeature : ScriptableRendererFeature
    {
        private const string BackFacePassLightMode = "GlassRefraction";
        private const string FrontFacePassLightMode = "GlassLit";
        private const string GrabbedTextureName = "_GrabTex";

        private RTHandle GrabbedTextureHandle { get; set; }
        
        private GrabTexturePass GrabTexturePass { get; set; }
        private GlassRefractionPass GlassRefractionPass { get; set; }
        private GlassLitPass GlassLitPass { get; set; }

        public override void Create()
        {
            GrabbedTextureHandle = RTHandles.Alloc(GrabbedTextureName, GrabbedTextureName);

            GrabTexturePass = new GrabTexturePass(GrabbedTextureName, GrabbedTextureHandle);
            GlassRefractionPass = new GlassRefractionPass(BackFacePassLightMode);
            GlassLitPass = new GlassLitPass(FrontFacePassLightMode);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            GrabTexturePass.BeforeEnqueue(renderer);
            renderer.EnqueuePass(GrabTexturePass);
            renderer.EnqueuePass(GlassRefractionPass);
            renderer.EnqueuePass(GlassLitPass);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (GrabbedTextureHandle != null) RTHandles.Release(GrabbedTextureHandle);
            }
            base.Dispose(disposing);
        }
    }
}
