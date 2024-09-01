using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace tana_gh.Mancala
{
    public class GrabTexturePass : ScriptableRenderPass
    {
        private string GrabbedTextureName { get; }
        private RTHandle GrabbedTextureHandle { get; set; }
        private int GrabbedTexturePropertyId { get; set; }
        private ScriptableRenderer Renderer { get; set; }

        public GrabTexturePass(string grabbedTextureName, RTHandle grabbedTextureHandle)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

            GrabbedTextureName = grabbedTextureName;
            GrabbedTextureHandle = grabbedTextureHandle;
            GrabbedTexturePropertyId = Shader.PropertyToID(grabbedTextureName);
        }

        public void BeforeEnqueue(ScriptableRenderer renderer)
        {
            Renderer = renderer;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(GrabbedTexturePropertyId, cameraTextureDescriptor);
            cmd.SetGlobalTexture(GrabbedTextureName, GrabbedTextureHandle);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get(nameof(GrabTexturePass));
            cmd.Clear();
            cmd.Blit(Renderer.cameraColorTargetHandle.rt, GrabbedTextureHandle);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
