public class TimePresenter
{
    readonly TimeModel _model;
    readonly TimeView _view;
    public TimePresenter(TimeModel model, TimeView view)
    {
        _model = model;
        _view = view;
    }
    public void Initialize()
    {
        UpdateView();
    }

    public void Update(float deltaTime)
    {
        if (_model.IsTimeOver)
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

    public bool GetIsTimeOver()
    {
        return _model.IsTimeOver;
    }

    public float GetCurrentTime()
    {
        return _model.CurrentTime;
    }
}
