using DG.Tweening;
using System;
using UnityEngine;

public class PanelUIView : MonoBehaviour
{
    [SerializeField]
    CanvasGroup _uiPanel;
    [SerializeField]
    float _duration = 0.5f;
    public void Fade(float alpha, Action onComplete)
    {
        _uiPanel.DOFade(alpha, _duration).OnComplete(() => onComplete?.Invoke());
    }
}
