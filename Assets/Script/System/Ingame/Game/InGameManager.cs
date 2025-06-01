using UnityEngine;

public class InGameManager : MonoBehaviour
{

    [SerializeField]
    PlayerManager _player;

    [SerializeField]
    int _maxTime;

    TimePresenter _timePresenter;

    bool _isPaused = true;
    public bool IsPaused => _isPaused;
    private void Awake()
    {
        ServiceLocator.Set(this);
    }

    void Start()
    {
        var timeModel = new TimeModel(_maxTime);

        var timeView = ServiceLocator.Get<GameUIManager>().TimeView;

        var resultModel = new GameResultModel();

        var resultView = ServiceLocator.Get<GameUIManager>().GameResultView;

        var gameResultPresenter = new GameResultPresenter(resultModel, resultView);

        var enemyManager = ServiceLocator.Get<EnemyManager>();

        var scoreManager = ServiceLocator.Get<ScoreManager>();

        _timePresenter = new TimePresenter(timeModel, timeView, _player, gameResultPresenter, enemyManager, scoreManager);

        _timePresenter?.Initialize();
    }

    void Update()
    {
        if (_isPaused) { return; }

        _timePresenter?.Update(Time.deltaTime);
    }

    public void PauseResume()
    {
        _isPaused = !_isPaused;
    }
}
