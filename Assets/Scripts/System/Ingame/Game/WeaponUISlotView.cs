using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class WeaponUISlotView : MonoBehaviour
{
    [SerializeField] private Text _bulletCountText;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _backGround;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;

    int _maxCount;
    public void SetContent(int count, Sprite icon)
    {
        _iconImage.sprite = icon;
        _maxCount = count;
        _bulletCountText.text = $"{count:000}/{_maxCount:000}";
    }

    public void SetCount(int count)
    {
        _bulletCountText.text = $"{count:000}/{_maxCount:000}";
    }

    public void Animate(bool enabled, float posX, float scale, float alpha, float duration)
    {
        _bulletCountText.enabled = enabled;
        _rectTransform.DOAnchorPosX(posX, duration);
        _rectTransform.DOScale(Vector3.one * scale, duration);
        _canvasGroup.DOFade(alpha, duration);

        if (!enabled) { return; }
        _rectTransform.SetAsLastSibling();
    }

    public void Reload(float duration)
    {
        // 一度fillを0にする
        _backGround.fillAmount = 0;

        // fillAmountを指定時間かけて1にするアニメーション
        _backGround.DOFillAmount(1, duration);
    }
}
