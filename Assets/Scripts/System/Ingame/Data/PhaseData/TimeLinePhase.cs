using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "TimeLinePhase", menuName = "GameData/PhaseData/TimeLinePhase")]
public class TimeLinePhase : PhaseData_B
{
    [Tooltip("TimeLineのアセット")]
    [SerializeField] private TimelineAsset _timeLine;
    [Tooltip("TimeLine終了時のイベント突入フラグ")]
    [SerializeField] private bool _isInEvent = false;
    public override async UniTask Run(PhaseContext context, CancellationToken token)
    {
        context.IngameManager.SetInEvent(true);

        var pd = context.TimeLineManager.PlayableDirector;

        pd.Play(_timeLine);

        await UniTask.WaitUntil(() => pd.state != PlayState.Playing, PlayerLoopTiming.Update, token);

        context.IngameManager.SetInEvent(_isInEvent);
    }
}
