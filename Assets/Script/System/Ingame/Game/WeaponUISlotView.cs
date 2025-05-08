using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class WeaponUISlotView : MonoBehaviour
{
    [SerializeField]
    Text _bulletCountText;
    [SerializeField]
    Image _iconImage;
    [SerializeField]
    RectTransform _rectTransform;
    [SerializeField]
    CanvasGroup _canvasGroup;
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
    public void AnimateToFront()
    {
        _bulletCountText.enabled = true;
        _rectTransform.DOAnchorPosX(100, 0.5f);
        _rectTransform.DOScale(Vector3.one * 5, 0.5f);
        _canvasGroup.DOFade(1, 0.5f);
        _rectTransform.SetAsLastSibling();
    }

    public void AnimateToBack()
    {
        _bulletCountText.enabled = false;
        _rectTransform.DOAnchorPosX(-100, 0.5f);
        _rectTransform.DOScale(Vector3.one * 3, 0.5f);
        _canvasGroup.DOFade(0.5f, 0.5f);
    }
}
