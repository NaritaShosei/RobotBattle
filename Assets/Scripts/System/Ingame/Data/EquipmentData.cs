using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentData
{
    [SerializeField] private PlayerEquipment _currentLoadout = new PlayerEquipment();
    [SerializeField] private List<int> _unlockedWeaponIDs = new List<int>();

    public PlayerEquipment CurrentLoadout => _currentLoadout;
    public List<int> UnlockedWeaponIds => _unlockedWeaponIDs;

    // 武器のアンロック
    public bool UnlockWeapon(int weaponId)
    {
        if (!_unlockedWeaponIDs.Contains(weaponId))
        {
            _unlockedWeaponIDs.Add(weaponId);
            return true;
        }
        return false;
    }

    // 装備変更
    public bool EquipWeapon(WeaponType type, int weaponId)
    {
        if (!_unlockedWeaponIDs.Contains(weaponId)) return false;

        switch (type)
        {
            case WeaponType.Main:
                _currentLoadout.PrimaryWeaponId = weaponId;
                break;
            case WeaponType.Sub:
                _currentLoadout.SecondWeaponId = weaponId;
                break;
        }

        return true;
    }
}

[System.Serializable]
public class PlayerEquipment
{
    [SerializeField] private int _primaryWeaponId;      // メイン武器ID
    [SerializeField] private int _secondWeaponId;      // サブ武器ID
    [SerializeField] private int _specialId;           // 必殺技ID

    public int PrimaryWeaponId
    {
        get => _primaryWeaponId;
        set => _primaryWeaponId = value;
    }

    public int SecondWeaponId
    {
        get => _secondWeaponId;
        set => _secondWeaponId = value;
    }

    public int SpecialID
    {
        get => _specialId;
        set => _specialId = value;
    }
}
