using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "FadePhase", menuName = "GameData/PhaseData/FadePhase")]
public class UIFadePhase : PhaseData_B
{
    [Tooltip("Fadeの初期値")]
    [SerializeField] private float _startAlpha = 0;
    [Tooltip("Fadeの目標値")]
    [SerializeField] private float _targetAlpha = 1;
    [Tooltip("Fade終了時のイベント突入フラグ")]
    [SerializeField] private bool _isInEvent = false;
    public override async UniTask Run(PhaseContext context, CancellationToken token)
    {
        context.IngameManager.SetInEvent(true);

        var fade = context.FadePanel;

        await fade.Fade(_startAlpha, _targetAlpha);

        context.IngameManager.SetInEvent(_isInEvent);
    }
}
