using Cysharp.Threading.Tasks;
using UnityEngine;

public class IngameManager : MonoBehaviour
{

    [SerializeField]
    PlayerManager _player;

    [SerializeField]
    int _maxTime;

    [SerializeField] private string _inputMode = "Player";

    TimePresenter _timePresenter;
    GameResultPresenter _gameResultPresenter;
    EnemyManager _enemyManager;
    ScoreManager _scoreManager;

    bool _isPaused;
    public bool IsPaused => _isPaused;

    bool _isGameEnd;
    public bool IsGameEnd => _isGameEnd;

    private bool _isInEvent;
    public bool IsInEvent => _isInEvent;
    private void Awake()
    {
        ServiceLocator.Set(this);
    }

    void Start()
    {
        ServiceLocator.Get<FadePanel>().Fade(0).Forget();

        var timeModel = new TimeModel(_maxTime);

        var timeView = ServiceLocator.Get<GameUIManager>().TimeView;

        var resultModel = new GameResultModel();

        var resultView = ServiceLocator.Get<GameUIManager>().GameResultView;

        _gameResultPresenter = new GameResultPresenter(resultModel, resultView);

        _enemyManager = ServiceLocator.Get<EnemyManager>();

        _scoreManager = ServiceLocator.Get<ScoreManager>();

        _timePresenter = new TimePresenter(timeModel, timeView);

        _timePresenter?.Initialize();

        ServiceLocator.Get<PhaseManager>().OnComplete += GameEnd;

        ServiceLocator.Get<InputManager>().SwitchInputMode(_inputMode);
    }

    private void GameEnd()
    {
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
    public void SetIsPause(bool value)
    {
        _isPaused = value;
    }

    public void SetInEvent(bool value)
    {
        _isInEvent = value;
    }
}
