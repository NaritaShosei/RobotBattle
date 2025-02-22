using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadUIManager : MonoBehaviour
{
    private UIDocument _document;
    private VisualElement _icon;

    private float _angle = 0;
    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _icon = _document.rootVisualElement.Q<VisualElement>("load-icon");
    }

    private void Update()
    {
        _angle -= Time.deltaTime * 360;
        _icon.style.rotate = new StyleRotate(new Rotate(_angle));
    }
}
