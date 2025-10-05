using UnityEngine;
using UnityEngine.UI;

public class SpecialGaugeView : MonoBehaviour
{
    [SerializeField] private Image _gaugeView;

    public void UpdateView(float amount)
    {
        _gaugeView.fillAmount = amount;
    }
}
