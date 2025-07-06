using Unity.VisualScripting;
using UnityEngine;

public class WeaponPresenter
{
    readonly WeaponView _view;

    public WeaponPresenter(WeaponView view)
    {
        _view = view;
    }
    public void Initialize(PlayerWeapon weaponA, PlayerWeapon weaponB)
    {
        _view.Initialize(weaponA, weaponB);
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
