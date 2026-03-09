using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Karbon {

public sealed class PadInputHandler : MonoBehaviour
{
    [field:SerializeField] public float ReleaseTime { get; set; } = 0.5f;
    [field:SerializeField, Min(1)] public float DecayExponent { get; set; } = 2.0f;
    [Space]
    [SerializeField] InputAction _input = null;
    [Space]
    [SerializeField] UnityEvent<float> _valueTarget = null;

    const float kHoldTime = 0.5f;

    float _currentValue;
    float _maxStrength;
    float _attenuationRate;

    double _startTime;
    float _currentStrength;
    bool _isPressed;

    void OnEnable()
    {
        if (_input == null) return;
        _input.started += OnStarted;
        _input.performed += OnPerformed;
        _input.canceled += OnCanceled;
        _input.Enable();
    }

    void OnDisable()
    {
        if (_input == null) return;
        _input.started -= OnStarted;
        _input.performed -= OnPerformed;
        _input.canceled -= OnCanceled;
        _input.Disable();
    }

    void OnStarted(InputAction.CallbackContext context)
    {
        _startTime = context.time;
        _maxStrength = 0;
        _isPressed = true;
    }

    void OnPerformed(InputAction.CallbackContext context)
    {
        _currentStrength = context.ReadValue<float>();
        _maxStrength = Mathf.Max(_maxStrength, _currentStrength);
    }

    void OnCanceled(InputAction.CallbackContext context)
    {
        _isPressed = false;
        var duration = (float)(context.time - _startTime);

        if (duration < kHoldTime)
        {
            // Case 1: Short press
            _currentValue = 1.0f;
            var decayTime = ReleaseTime * Mathf.Pow(_maxStrength, DecayExponent);
            _attenuationRate = (decayTime > 1e-5f) ? (1.0f / decayTime) : float.MaxValue;
        }
        else
        {
            // Case 2: Long press
            // _currentValue starts decaying from the current strength at release.
            _attenuationRate = (ReleaseTime > 1e-5f) ? (1.0f / ReleaseTime) : float.MaxValue;
        }
    }

    void Update()
    {
        if (_isPressed)
        {
            _currentValue = _currentStrength;
        }
        else if (_currentValue > 0)
        {
            _currentValue -= _attenuationRate * Time.deltaTime;
            if (_currentValue < 0) _currentValue = 0;
        }

        _valueTarget?.Invoke(_currentValue);
    }
}

} // namespace Karbon
