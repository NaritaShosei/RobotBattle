using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BasicButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("ボタンアニメーション設定")]
    [SerializeField] private float _clickScale = 0.8f;
    [SerializeField] private float _clickDuration = 0.15f;
    [SerializeField] private float _enterScale = 1.2f;
    [SerializeField] private float _exitScale = 1f;
    [SerializeField] private float _enterExitScaleDuration = 0.3f;
    private Sequence _clickSequence;

    public event Action OnClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        _clickSequence.Kill();

        _clickSequence = DOTween.Sequence();
        _clickSequence.Append(transform.DOScale(Vector3.one * _clickScale, _clickDuration)).
            Append(transform.DOScale(Vector3.one * _enterScale, _clickDuration)).
            OnComplete(() => OnClick?.Invoke());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one * _enterScale, _enterExitScaleDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one * _exitScale, _enterExitScaleDuration);
    }
}
