using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISubmitPanel : UISubmitBase
{
    [SerializeField] UnityEvent _events;
    [SerializeField] TargetType _targetType;
    [SerializeField, Range(0, 1)] float _fadeAlpha = 0;
    [SerializeField] float _fadeDuration = 0.5f;
    [SerializeField] Vector3 _selectScale = Vector3.one;
    [SerializeField] Vector3 _exitScale = Vector3.one;
    [SerializeField] Vector3 _submitScale = Vector3.one;
    [SerializeField] float _scaleDuration = 0.5f;
    Image _image;
    [SerializeField] Color _submitColor = Color.yellow;
    [SerializeField] Color _selectColor = Color.red;
    Color _baseColor = Color.white;

    private void OnEnable()
    {
        OnMouseEnter += Select;
        OnMouseExit += Exit;
        OnClick += SubmitView;
        if (TryGetComponent(out _image))
        {
            _baseColor = _image.color;
        }
    }

    private void OnDisable()
    {
        OnMouseEnter -= Select;
        OnMouseExit -= Exit;
        OnClick -= SubmitView;
    }

    public override void Submit()
    {
        IsClicked = true;
        if (_targetType == TargetType.None) { _events.Invoke(); return; }

        var view = ServiceLocator.Get<GameUIManager>().PanelUIView;

        view.Fade(_targetType, _fadeAlpha, _fadeDuration, () => _events?.Invoke());
        Debug.Log("UI Submit");
    }

    public void Select()
    {
        transform.DOScale(_selectScale, _scaleDuration).SetLink(gameObject);
        _image.color = _selectColor;
    }
    public void Exit()
    {
        transform.DOScale(_exitScale, _scaleDuration).SetLink(gameObject);
        _image.color = _baseColor;
    }
    public void SubmitView()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(_submitScale, _scaleDuration)).Append(transform.DOScale(_exitScale, _scaleDuration)).OnComplete(() => Submit()).SetLink(gameObject);
        _image.color = _submitColor;
    }
}