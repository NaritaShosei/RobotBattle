using UnityEngine;
using UnityEngine.Events;

public class UISubmitPanel : UISubmitBase
{
    [SerializeField] UnityEvent _events;
    public override void Submit()
    {
        //TODO : DOFade とかやりたい
        ServiceLocator.Get<GameUIManager>().PanelUIView.Fade(0, () => _events?.Invoke());
        Debug.Log("UI Submit");
    }
}
