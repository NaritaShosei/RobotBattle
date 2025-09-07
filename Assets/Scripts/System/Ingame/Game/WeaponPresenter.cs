using Unity.VisualScripting;
using UnityEngine;

public class WeaponPresenter
{
    readonly WeaponView _view;

    public WeaponPresenter(WeaponView view)
    {
        _view = view;
    }
    public void Initialize((int count, Sprite icon) weaponA, (int count, Sprite icon) weaponB)
    {
        _view.Initialize((weaponA.count, weaponA.icon), (weaponB.count, weaponB.icon));
    }

    public void CountUpdate(int count)
    {
        _view.CountUpdate(count);
    }

    public void SwapWeapon()
    {
        _view.Swap();
    }
}
