using UnityEngine;

public class IngameManager : MonoBehaviour
{

    [SerializeField]
    PlayerManager _player;

    [SerializeField]
    int _maxTime;

    TimePresenter _timePresenter;
    GameResultPresenter _gameResultPresenter;
    EnemyManager _enemyManager;
    ScoreManager _scoreManager;

    bool _isPaused = true;
    public bool IsPaused => _isPaused;

    bool _isGameEnd;
    public bool IsGameEnd => _isGameEnd;
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

        _gameResultPresenter = new GameResultPresenter(resultModel, resultView);

        _enemyManager = ServiceLocator.Get<EnemyManager>();

        _scoreManager = ServiceLocator.Get<ScoreManager>();

        _timePresenter = new TimePresenter(timeModel, timeView);

        _timePresenter?.Initialize();
    }

    void Update()
    {
        return;
        if (_isGameEnd) { return; }

        if (_isPaused) { return; }

        _timePresenter?.Update(Time.deltaTime);

        if (_timePresenter.GetIsTimeOver())
        {
            _gameResultPresenter.SetGameOver(GameOverType.TimeOver, _scoreManager.Score);
            _isGameEnd = true;
            ServiceLocator.Get<InputManager>().SwitchInputMode(InputManager.UI);
        }

        if (_player.IsState(PlayerState.Dead))
        {
            _gameResultPresenter.SetGameOver(GameOverType.Death, _scoreManager.Score);
            _isGameEnd = true;
            ServiceLocator.Get<InputManager>().SwitchInputMode(InputManager.UI);
        }

        if (_enemyManager.IsEnemyAllDefeated)
        {
            _gameResultPresenter.SetGameClear(_timePresenter.GetCurrentTime(), _scoreManager.Score);
            _isGameEnd = true;
            ServiceLocator.Get<InputManager>().SwitchInputMode(InputManager.UI);
        }
    }

    [ContextMenu(nameof(PauseResume))]
    public void PauseResume()
    {
        _isPaused = !_isPaused;
    }
}
