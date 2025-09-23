using System;
using UnityEngine;

public class SpecialGauge
{
    private float _value;
    private float _max = 100;
    public event Action<float> OnValueChange;
    public event Action OnUseGauge;
    public event Action OnGaugeMax;
    public bool IsGaugeMax => _value >= _max;

    public void Initialize(float max)
    {
        _max = max;
    }

    public void UpdateValue(float value)
    {
        if (IsGaugeMax) { return; }

        var oldValue = _value;

        _value = Mathf.Min(value, _max);
        OnValueChange?.Invoke(_value / _max);

        if (IsGaugeMax)
        {
            OnGaugeMax?.Invoke();
        }
    }

    public bool TryUseGauge()
    {
        if (!IsGaugeMax) { return false; }

        OnUseGauge?.Invoke();
        _value = 0;

        return true;
    }
}
