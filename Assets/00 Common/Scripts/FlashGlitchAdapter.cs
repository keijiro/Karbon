using UnityEngine;

namespace Karbon {

public sealed class FlashGlitchAdapter : MonoBehaviour
{
    [SerializeField] Vector2 _trigger1MinMax = new Vector2(0.1f, 0.5f);
    [SerializeField] Vector2 _trigger2MinMax = new Vector2(0.1f, 0.5f);
    [SerializeField] FlashGlitch.FlashGlitchController _target = null;

    public void Trigger1(float strength)
    {
        _target.ReleaseTime1 = strength + 0.1f;
        _target.TriggerEffect1(0.5f + strength * 0.5f);
    }

    public void Trigger2(float strength)
    {
        _target.ReleaseTime2 = strength + 0.1f;
        _target.TriggerEffect2(0.5f + strength * 0.5f);
        if (Random.value < 0.1f) _target.RandomizeHue();
    }
}

} // namespace Karbon
