using UnityEngine;

public class InGameManager : MonoBehaviour
{

    [SerializeField]
    PlayerManager _player;

    [SerializeField]
    int _maxTime;

    TimePresenter _timePresenter;

    void Start()
    {
        var model = new TimeModel(_maxTime);

        var view = GameUIManager.Instance.TimeView;

        _timePresenter = new TimePresenter(model, view, _player);
    }

    // Update is called once per frame
    void Update()
    {
        _timePresenter?.Update(Time.deltaTime);
    }
}
