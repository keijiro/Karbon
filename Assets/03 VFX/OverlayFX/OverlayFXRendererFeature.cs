using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace URPCameraEffect {

public sealed class OverlayFXRendererFeature : ScriptableRendererFeature
{
    [SerializeField]
    RenderPassEvent _passEvent = RenderPassEvent.AfterRenderingPostProcessing;

    OverlayFXPass _pass;

    public override void Create()
      => _pass = new OverlayFXPass { renderPassEvent = _passEvent };

    public override void AddRenderPasses
      (ScriptableRenderer renderer, ref RenderingData renderingData)
      => renderer.EnqueuePass(_pass);
}

} // namespace URPCameraEffect
