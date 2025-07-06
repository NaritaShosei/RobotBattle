using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class GaugeView : MonoBehaviour
{
    [SerializeField]
    Image _backGauge;
    [SerializeField]
    Image _gauge;
    Sequence _sequence;

    float _maxGauge;

    public void Initialize(float maxGauge)
    {
        _maxGauge = maxGauge;
    }

    public void GaugeUpdate(float currentValue)
    {
        float value = currentValue / _maxGauge;
        _gauge.fillAmount = value;

        _sequence?.Kill();

        _sequence = DOTween.Sequence().
                    Append(_backGauge.DOFillAmount(value, 0.5f));
    }
}
