using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISubmitPanel : UISubmitBase
{
    [SerializeField] UnityEvent _events;
    [SerializeField] Image _image;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField, Range(0, 1)] float _alpha = 0;
    [SerializeField] float _duration = 0.5f;
    public override void Submit()
    {
        ServiceLocator.Get<GameUIManager>().PanelUIView.Fade(_canvasGroup, _alpha, _duration, () => _events?.Invoke());
        ServiceLocator.Get<GameUIManager>().PanelUIView.Fade(_image, _alpha, _duration, () => _events?.Invoke());
        Debug.Log("UI Submit");
    }
}