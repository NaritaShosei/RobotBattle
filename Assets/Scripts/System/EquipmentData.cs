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
    public bool EquipWeapon(EquipmentType type, int weaponId)
    {
        if (!_unlockedWeaponIDs.Contains(weaponId)) return false;

        switch (type)
        {
            case EquipmentType.Main:
                _currentLoadout.PrimaryWeaponId = weaponId;
                break;
            case EquipmentType.Sub:
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
}
