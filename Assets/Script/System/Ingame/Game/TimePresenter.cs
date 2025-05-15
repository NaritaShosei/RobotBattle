using UnityEngine;

public class TimePresenter
{
    readonly TimeModel _model;
    readonly TimeView _view;
    readonly PlayerManager _player;
    readonly GameResultPresenter _resultPresenter;

    public TimePresenter(TimeModel model, TimeView view, PlayerManager player, GameResultPresenter resultPresenter)
    {
        _model = model;
        _view = view;
        _player = player;
        _resultPresenter = resultPresenter;
    }

    public void Update(float deltaTime)
    {
        if (_model.IsTimeOver)
        {
            return;
        }

        if (_player.State == PlayerState.Dead)
        {
            _resultPresenter.SetGameOver(GameOverType.Death, ScoreManager.Instance.Score);
            return;
        }

        if (EnemyManager.Instance.IsEnemyAllDefeated)
        {
            _resultPresenter.SetGameClear(_model.CurrentTime, ScoreManager.Instance.Score);
            return;
        }

        _model.UpdateTime(deltaTime);
        UpdateView();

        if (_model.IsTimeOver)
        {
            //時間切れになった際のコールバックなど
            _resultPresenter.SetGameOver(GameOverType.TimeOver, ScoreManager.Instance.Score);
        }
    }

    void UpdateView()
    {
        _view.SetTime(_model.CurrentTime);
    }
}
