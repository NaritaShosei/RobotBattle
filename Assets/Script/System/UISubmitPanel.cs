using UnityEngine;
using UnityEngine.Events;

public class UISubmitPanel : UISubmitBase
{
    [SerializeField] UnityEvent _events;
    public override void Submit()
    {
        _events?.Invoke();
        Debug.Log("UI Submit");
    }
}
