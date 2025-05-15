using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameResultPresenter
{
    GameResultModel _model;
    GameResultView _view;

    public GameResultPresenter(GameResultModel model, GameResultView view)
    {
        _model = model;
        _view = view;
    }

    public void SetGameOver(GameOverType gameOverType, int score)
    {
        _model.SetGameOver(gameOverType);
        _model.SetScore(score);
        UpdateView();
    }

    public void SetGameClear(float time, int score)
    {
        _model.SetGameClear();
        _model.SetTime(time);
        _model.SetScore(score);
        UpdateView();
    }

    void UpdateView()
    {
        _view.ShowUI();
        if (_model.IsGameOver)
        {
            _view.SetGameOverType(_model.GameOverType);
            _view.SetScore(_model.Score);
        }
        else if (_model.IsGameClear)
        {
            _view.SetGameClear();
            _view.SetScore(_model.Score);
            _view.SetTime(_model.ClearTime);
        }
    }
}
