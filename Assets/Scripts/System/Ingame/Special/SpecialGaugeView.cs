using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SpecialGaugeView : MonoBehaviour
{
    [SerializeField] private Image _gaugeView;
    [Header("UIアニメーション設定")]
    [SerializeField] private float _duration = 0.2f;
    [SerializeField] private Color _color = Color.yellow;
    [SerializeField] private float _colorDuration = 0.2f;
    private Color _baseColor;

    private void Awake()
    {
        _baseColor = _gaugeView.color;
    }

    public void UpdateView(float amount)
    {
        _gaugeView.fillAmount = amount;
    }

    public void UseGauge()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(_gaugeView.DOFillAmount(0, _duration)).
            Append(_gaugeView.DOColor(_baseColor, _colorDuration)).
            SetLink(gameObject);

        seq.Play();
    }

    public void MaxGauge()
    {
        _gaugeView.DOColor(_color, _colorDuration).SetLink(gameObject);
    }
}
