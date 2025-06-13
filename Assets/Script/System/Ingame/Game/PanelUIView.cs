using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PanelUIView : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] CanvasGroup _canvasGroup;

    public void Fade(TargetType type, float alpha, float duration, Action onComplete = null)
    {
        switch (type)
        {
            case TargetType.Image:
                _image.DOFade(alpha, duration).OnComplete(() => onComplete?.Invoke());
                break;
            case TargetType.CanvasGroup:
                _canvasGroup.DOFade(alpha, duration).OnComplete(() => onComplete?.Invoke());
                break;
        }

    }
}
public enum TargetType
{
    CanvasGroup,
    Image,
}