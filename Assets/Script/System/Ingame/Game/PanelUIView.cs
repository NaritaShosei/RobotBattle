using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PanelUIView : MonoBehaviour
{
    public void Fade(CanvasGroup fadeTarget, float alpha, float duration, Action onComplete = null)
    {
        if (!fadeTarget) return;
        fadeTarget.DOFade(alpha, duration).OnComplete(() => onComplete?.Invoke());
    }
    public void Fade(Image fadeTarget, float alpha, float duration, Action onComplete = null)
    {
        if (!fadeTarget) return;
        fadeTarget.DOFade(alpha, duration).OnComplete(() => onComplete?.Invoke());
    }
}
