using UnityEngine;

public class TimePresenter
{
    readonly TimeModel _model;
    readonly TimeView _view;
    readonly PlayerManager _player;
    readonly GameResultPresenter _resultPresenter;
    readonly EnemyManager _enemyManager;
    readonly ScoreManager _scoreManager;
    public TimePresenter(TimeModel model, TimeView view, PlayerManager player, GameResultPresenter resultPresenter, EnemyManager enemyManager, ScoreManager scoreManager)
    {
        _model = model;
        _view = view;
        _player = player;
        _resultPresenter = resultPresenter;
        _enemyManager = enemyManager;
        _scoreManager = scoreManager;
    }
    public void Initialize()
    {
        UpdateView();
    }

    public void Update(float deltaTime)
    {
        if (_model.IsTimeOver)
        {
            return;
        }

        if (_player.State == PlayerState.Dead)
        {
            _resultPresenter.SetGameOver(GameOverType.Death, _scoreManager.Score);
            return;
        }

        if (_enemyManager.IsEnemyAllDefeated)
        {
            _resultPresenter.SetGameClear(_model.CurrentTime, _scoreManager.Score);
            return;
        }

        _model.UpdateTime(deltaTime);
        UpdateView();

        if (_model.IsTimeOver)
        {
            //時間切れになった際のコールバックなど
            _resultPresenter.SetGameOver(GameOverType.TimeOver, _scoreManager.Score);
        }
    }

    void UpdateView()
    {
        _view.SetTime(_model.CurrentTime);
    }
}
