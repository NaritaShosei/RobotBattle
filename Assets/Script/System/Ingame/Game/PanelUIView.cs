using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PanelUIView : MonoBehaviour
{
    [SerializeField] Image _fadePanel;
    [SerializeField] CanvasGroup _startPanel;
    [SerializeField] CanvasGroup _settingPanel;

    public void Fade(TargetType type, float alpha, float duration, Action onComplete = null)
    {
        switch (type)
        {
            case TargetType.Image:
                _fadePanel.DOFade(alpha, duration).OnComplete(() => onComplete?.Invoke());
                break;

            case TargetType.CanvasGroup:
                _startPanel.DOFade(alpha, duration).OnComplete(() => onComplete?.Invoke());
                break;

            case TargetType.Setting:
                _settingPanel.DOFade(alpha, duration).OnComplete(() => onComplete?.Invoke());
                break;
        }

    }
}
public enum TargetType
{
    None,
    CanvasGroup,
    Image,
    Setting,
}