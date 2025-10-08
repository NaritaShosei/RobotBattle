using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "TimeLinePhase", menuName = "GameData/PhaseData/TimeLinePhase")]
public class TimeLinePhase : PhaseData_B
{
    [SerializeField] private TimelineAsset _timeLine;
    public override async UniTask Run(PhaseContext context, CancellationToken token)
    {
        var pd = context.TimeLineManager.PlayableDirector;

        pd.Play(_timeLine);

        await UniTask.WaitUntil(() => pd.state != PlayState.Playing, PlayerLoopTiming.Update, token);
    }
}
