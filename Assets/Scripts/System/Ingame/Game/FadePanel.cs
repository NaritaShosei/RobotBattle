using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour
{
    [SerializeField] private Image _fadePanel;
    [SerializeField] private float _duration = 0.5f;

    private void Awake()
    {
        ServiceLocator.Set(this);
    }

    public async UniTask Fade(float alpha)
    {
        await _fadePanel.DOFade(alpha, _duration).AsyncWaitForCompletion();
    }
}