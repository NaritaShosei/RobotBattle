using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    private EquipmentData _playerData;

    private void Awake()
    {
        ServiceLocator.Set(this);
        _playerData = SaveLoadService.Load<EquipmentData>();
    }

    public void SelectWeapon(int id)
    {
        _playerData.EquipWeapon(id);

        SaveLoadService.Save(_playerData);
    }
}
