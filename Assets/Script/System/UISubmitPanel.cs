using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISubmitPanel : UISubmitBase
{
    [SerializeField] UnityEvent _events;
    [SerializeField] TargetType _targetType;
    [SerializeField, Range(0, 1)] float _alpha = 0;
    [SerializeField] float _duration = 0.5f;
    public override void Submit()
    {
        var view = ServiceLocator.Get<GameUIManager>().PanelUIView;

        view.Fade(_targetType, _alpha, _duration, () => _events?.Invoke());
        Debug.Log("UI Submit");
    }
}