using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class PhaseManager : MonoBehaviour
{
    [SerializeField] private PhaseDataBase _phaseDataBase;

    private PhaseContext _context;

    private async void Start()
    {
        _context = new PhaseContext();

        _context.TimeLineManager = ServiceLocator.Get<TimeLineManager>();

        await WaitAllPhase();
    }

    private async UniTask WaitAllPhase()
    {
        foreach (var phase in _phaseDataBase.AllPhaseData)
        {
            Debug.Log($"{phase.PhaseName}==Start Run");

            await phase.Run(_context);
        }

        Debug.Log("End");
    }
}
// フェーズ間で共有したいデータなど
public class PhaseContext
{
    public TimeLineManager TimeLineManager;
}