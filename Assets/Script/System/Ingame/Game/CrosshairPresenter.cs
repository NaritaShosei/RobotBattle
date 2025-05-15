using UnityEngine;

public class CrosshairPresenter
{
    readonly CrosshairView _view;

    public CrosshairPresenter(CrosshairView view)
    {
        _view = view;
    }

    public void Initialize()
    {
        _view.SetLockOn(false);
    }

    public void UpdateLockOn(bool isLocked, Vector3 targetPos)
    {
        _view.SetLockOn(isLocked);

        if (isLocked)
        {
            _view.SetLockOnPosition(targetPos);
        }
    }

}
