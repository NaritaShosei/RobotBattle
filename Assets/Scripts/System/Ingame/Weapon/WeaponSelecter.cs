using System.Collections.Generic;
using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    private EquipmentData _playerData;
    public EquipmentData PlayerData => _playerData;

    private void Awake()
    {
        ServiceLocator.Set(this);
        _playerData = SaveLoadService.Load<EquipmentData>();
    }

    public bool SelectWeapon(EquipmentType type, int id)
    {
        bool result = _playerData.EquipWeapon(type, id);

        SaveLoadService.Save(_playerData);

        return result;
    }

    public List<int> GetUnlockIDs() => _playerData.UnlockedWeaponIds;
}
