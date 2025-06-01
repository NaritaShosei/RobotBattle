using UnityEngine;

public class InGameManager : MonoBehaviour
{

    [SerializeField]
    PlayerManager _player;

    [SerializeField]
    int _maxTime;

    TimePresenter _timePresenter;
    GameResultPresenter _gameResultPresenter;

    void Start()
    {
        var timeModel = new TimeModel(_maxTime);

        var timeView = ServiceLocator.Get<GameUIManager>().TimeView;

        var resultModel = new GameResultModel();

        var resultView = ServiceLocator.Get<GameUIManager>().GameResultView;

        _gameResultPresenter = new GameResultPresenter(resultModel, resultView);

        _timePresenter = new TimePresenter(timeModel, timeView, _player, _gameResultPresenter);
    }

    void Update()
    {
        _timePresenter?.Update(Time.deltaTime);
    }
}
