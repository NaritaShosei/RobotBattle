using System;
using UnityEngine;

public class SpecialGaugeModel
{
    private float _value;
    private float _max = 100;
    public event Action<float> OnValueChange;
    public event Action OnUseGauge;
    public event Action OnGaugeMax;
    public bool IsGaugeMax => _value >= _max;
    public float Max => _max;
    public float CurrentValue => _value;

    public void Initialize(float max)
    {
        _max = max;
    }

    public void UpdateValue(float value)
    {
        if (IsGaugeMax) { return; }

        var oldValue = _value;

        _value = Mathf.Min(_value + value, _max);
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

public class SpecialGaugePresenter
{
    private SpecialGaugeModel _model;
    private SpecialGaugeView _view;

    public SpecialGaugePresenter(SpecialGaugeModel model, SpecialGaugeView view)
    {
        _model = model;
        _view = view;

        _model.OnValueChange += _view.UpdateView;
        _model.OnUseGauge += _view.UseGauge;
        _model.OnGaugeMax += _view.MaxGauge;

        _view.UpdateView(_model.CurrentValue / _model.Max);
    }
}