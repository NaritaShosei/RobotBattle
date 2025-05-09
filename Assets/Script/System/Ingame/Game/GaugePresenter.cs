using UnityEngine;

public class GaugePresenter
{
    GaugeView _view;

    public GaugePresenter(GaugeView gaugeView)
    {
        _view = gaugeView;
    }

    public void Initialize(float maxGauge)
    {
        _view.Initialize(maxGauge);
    }

    public void GaugeUpdate(float value)
    {
        _view.GaugeUpdate(value);
    }
}
