using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    ScorePresenter _presenter;
    int _score;
    public int Score => _score;

    private void Awake()
    {
        ServiceLocator.Set(this);
    }

    void Start()
    {
        _presenter = new ScorePresenter(ServiceLocator.Get<GameUIManager>().ScoreView);
        _presenter.ScoreUpdate(_score);
    }

    public void AddScore(int score)
    {
        _score += score;
        _presenter.ScoreUpdate(_score);
    }
}
