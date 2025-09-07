using UnityEngine;

public class ScorePresenter
{
    ScoreView _scoreView;

    public ScorePresenter(ScoreView scoreView)
    {
        _scoreView = scoreView;
    }

    public void ScoreUpdate(int score)
    {
        _scoreView.SetScore(score);
    }
}
