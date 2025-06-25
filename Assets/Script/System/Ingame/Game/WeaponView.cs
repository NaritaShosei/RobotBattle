using UnityEngine;

public class WeaponView : MonoBehaviour
{
    [SerializeField]
    WeaponUISlotView _slotA;

    [SerializeField]
    WeaponUISlotView _slotB;

    [SerializeField] float _frontPosX = 100;
    [SerializeField] float _backPosX = -100;
    [SerializeField] float _duration = 0.5f;
    [SerializeField] float _frontScale = 1.2f;
    [SerializeField] float _backScale = 0.8f;
    [SerializeField] float _frontAlpha = 1;
    [SerializeField] float _backAlpha = 0.5f;

    bool _isAFront;

    public void Initialize(PlayerWeapon weaponA, PlayerWeapon weaponB)
    {
        _slotA.SetContent(weaponA.Count, weaponA.Icon);
        _slotB.SetContent(weaponB.Count, weaponB.Icon);


        _slotA.Animate(true, _frontPosX, _frontScale, _frontAlpha, _duration);
        _slotB.Animate(false, _backPosX, _backScale, _backAlpha, _duration);
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
            _slotA.Animate(false, _backPosX, _backScale, _backAlpha, _duration);
            _slotB.Animate(true, _frontPosX, _frontScale, _frontAlpha, _duration);
        }

        else
        {
            _slotA.Animate(true, _frontPosX, _frontScale, _frontAlpha, _duration);
            _slotB.Animate(false, _backPosX, _backScale, _backAlpha, _duration);
        }

        _isAFront = !_isAFront;
    }
}
