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
        if (_player.State == PlayerState.Dead || _model.IsTimeOver)
        {
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
