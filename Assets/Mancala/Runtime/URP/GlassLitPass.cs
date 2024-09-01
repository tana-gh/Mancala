using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace tana_gh.Mancala
{
    public class GlassLitPass : ScriptableRenderPass
    {
        private ShaderTagId ShaderTagId { get; }

        public GlassLitPass(string shaderLightMode)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents + 2;

            ShaderTagId = new ShaderTagId(shaderLightMode);
        }

        private FilteringSettings _filteringSettings = new(RenderQueueRange.all);

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            cmd.Clear();
            var drawingSettings = CreateDrawingSettings(ShaderTagId, ref renderingData, SortingCriteria.CommonTransparent);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
