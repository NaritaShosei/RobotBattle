using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    ScorePresenter _presenter;
    int _score;
    public static ScoreManager Instance { get; private set; }
    void Start()
    {
        Instance = this;
        _presenter = new ScorePresenter(GameUIManager.Instance.ScoreView);
        _presenter.ScoreUpdate(_score);
    }

    public void AddScore(int score)
    {
        _score += score;
        _presenter.ScoreUpdate(_score);
    }
}
