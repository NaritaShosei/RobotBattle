using UnityEngine;

public class HPGaugePresenter
{
    HPGaugeView _view;

    public HPGaugePresenter(HPGaugeView view)
    {
        _view = view;
    }

    public void Initialize(float maxHealth)
    {
        _view.Initialize(maxHealth);
    }

    public void GaugeUpdate(float currentHealth)
    {
        _view?.GaugeUpdate(currentHealth);
    }
}
