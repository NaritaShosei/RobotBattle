﻿using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    ScorePresenter _presenter;
    int _score;
    public int Score => _score;
    public static ScoreManager Instance { get; private set; }
    void Start()
    {
        Instance = this;
        _presenter = new ScorePresenter(ServiceLocator.Get<GameUIManager>().ScoreView);
        _presenter.ScoreUpdate(_score);
    }

    public void AddScore(int score)
    {
        _score += score;
        _presenter.ScoreUpdate(_score);
    }
}
