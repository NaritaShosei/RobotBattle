using UnityEngine;

public class TimePresenter
{
    readonly TimeModel _model;
    readonly TimeView _view;
    readonly PlayerManager _player;

    public TimePresenter(TimeModel model, TimeView view, PlayerManager player)
    {
        _model = model;
        _view = view;
        _player = player;
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
        }
    }

    void UpdateView()
    {
        _view.SetTime(_model.CurrentTime);
    }
}
