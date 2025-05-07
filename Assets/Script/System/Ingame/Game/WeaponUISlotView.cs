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

    public void SetContent(int count, Sprite icon)
    {
        _iconImage.sprite = icon;
        _bulletCountText.text = count.ToString("000");
    }

    public void SetCount(int count)
    {
        _bulletCountText.text = count.ToString("000");
    }
    public void AnimateToFront()
    {
        _rectTransform.DOAnchorPosX(100, 0.5f);
        _rectTransform.DOScale(Vector3.one * 5, 0.5f);
        _canvasGroup.DOFade(1, 0.5f);
        _rectTransform.SetAsLastSibling();
    }

    public void AnimateToBack()
    {
        _rectTransform.DOAnchorPosX(-100, 0.5f);
        _rectTransform.DOScale(Vector3.one * 3, 0.5f);
        _canvasGroup.DOFade(0.5f, 0.5f);
    }
}
