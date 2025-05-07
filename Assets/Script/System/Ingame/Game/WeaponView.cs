using UnityEngine;

public class WeaponView : MonoBehaviour
{
    [SerializeField]
    WeaponUISlotView _slotA;

    [SerializeField]
    WeaponUISlotView _slotB;

    bool _isAFront;

    public void Initialize(PlayerWeapon weaponA, PlayerWeapon weaponB)
    {
        _slotA.SetContent(weaponA.Count, weaponA.Icon);
        _slotB.SetContent(weaponB.Count, weaponB.Icon);


        _slotA.AnimateToFront();
        _slotB.AnimateToBack();
        _isAFront = true;
    }
    public void CountUpdate(int count)
    {
        if (_isAFront)
        {
            _slotA.SetCount(count);
        }
        else
        {
            _slotB.SetCount(count);
        }
    }

    public void Swap()
    {
        if (_isAFront)
        {
            _slotA.AnimateToBack();
            _slotB.AnimateToFront();
        }

        else
        {
            _slotA.AnimateToFront();
            _slotB.AnimateToBack();
        }

        _isAFront = !_isAFront;
    }
}
