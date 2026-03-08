using UnityEngine;
using UnityEngine.Rendering;

namespace URPCameraEffect {

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("URP Camera Effect/Overlay FX")]
public sealed class OverlayFXController : MonoBehaviour
{
    [field:SerializeField, Range(0, 1)] public float Opacity { get; set; } = 1;

    [SerializeField, HideInInspector] Shader _shader = null;

    public bool IsActive => Opacity > 0;

    Material _material;

    public Material UpdateMaterial()
    {
        if (_material == null) _material = CoreUtils.CreateEngineMaterial(_shader);
        _material.SetFloat("_Opacity", Opacity);
        return _material;
    }

    void ReleaseResources()
    {
        CoreUtils.Destroy(_material);
        _material = null;
    }

    void OnDestroy() => ReleaseResources();
    void OnDisable() => ReleaseResources();
}

} // namespace URPCameraEffect
