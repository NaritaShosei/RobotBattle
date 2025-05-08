using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HPGaugeView : MonoBehaviour
{
    [SerializeField]
    Image _backGauge;
    [SerializeField]
    Image _gauge;
    Sequence _sequence;

    float _maxHealth;

    public void Initialize(float maxHealth)
    {
        _maxHealth = maxHealth;
    }

    public void GaugeUpdate(float currentValue)
    {
        float value = currentValue / _maxHealth;
        _gauge.fillAmount = value;

        _sequence?.Kill();

        //Enemyにも使えるようにSetLinkを使う
        _sequence = DOTween.Sequence().
                    SetLink(gameObject).
                    SetDelay(0.75f).
                    Append(_backGauge.DOFillAmount(value, 0.5f));
    }
}
