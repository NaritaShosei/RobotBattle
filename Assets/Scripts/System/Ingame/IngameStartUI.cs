using DG.Tweening;
using UnityEngine;

public class IngameStartUI : MonoBehaviour
{
    [SerializeField] private BasicButton _startButton;
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private float _fadeDuration = 0.3f;

    private void Start()
    {
        _startButton.OnClick += OnStart;
    }

    private void OnStart()
    {
        _group.DOFade(0, _fadeDuration).OnComplete(() =>
        {
            ServiceLocator.Get<InputManager>().SwitchInputMode(InputManager.PLAYER);
            ServiceLocator.Get<IngameManager>().PauseResume();
        });
    }
}
