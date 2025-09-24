using Unity.VisualScripting;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    private int _mainID;
    private int _subID;
    private int _specialID;


    private void Awake()
    {
        ServiceLocator.Set(this);
        var data = SaveLoadService.Load<EquipmentData>();
        _mainID = data.CurrentLoadout.PrimaryWeaponId;
        _subID = data.CurrentLoadout.SecondWeaponId;
        _specialID = data.CurrentLoadout.SpecialID;
    }

    public WeaponBase SpawnMainWeapon(PlayerEquipmentManager playerEquipmentManager)
    {
        var weaponData = ServiceLocator.Get<WeaponManager>().DataBase.GetWeapon(_mainID);

        if (weaponData?.WeaponPrefab == null) { Debug.LogWarning("プレハブが設定されていません"); return null; }

        var parent = playerEquipmentManager.GetEquipmentParent(weaponData.EquipmentType);

        return SpawnWeapon(weaponData, parent);
    }

    public WeaponBase SpawnSubWeapon(Transform parent)
    {
        var weaponData = ServiceLocator.Get<WeaponManager>().DataBase.GetWeapon(_subID);

        if (weaponData?.WeaponPrefab == null) { Debug.LogWarning("プレハブが設定されていません"); return null; }

        return SpawnWeapon(weaponData, parent);
    }

    private WeaponBase SpawnWeapon(WeaponData weaponData, Transform parent)
    {
        var weaponObj = Instantiate(weaponData.WeaponPrefab, parent);

        if (!weaponObj.TryGetComponent(out WeaponBase weaponComponent))
        {
            Debug.LogWarning("プレハブにWeaponBaseがアタッチされていません"); return null;
        }

        weaponComponent.Initialize(weaponData);

        return weaponComponent;
    }

    public SpecialAttackBase SpawnSpecial(PlayerEquipmentManager playerEquipmentManager)
    {
        var specialData = ServiceLocator.Get<WeaponManager>().DataBase.GetSpecial(_specialID);

        if (specialData?.Prefab == null) { Debug.LogWarning("プレハブが設定されていません"); return null; }

        var parent = playerEquipmentManager.GetEquipmentParent(specialData.EquipmentType);

        var specialObj = Instantiate(specialData.Prefab, parent);

        if (!specialObj.TryGetComponent(out SpecialAttackBase specialAttack))
        {
            Debug.LogWarning("プレハブにWeaponBaseがアタッチされていません"); return null;
        }

        specialAttack.Initialize(specialData);

        return specialAttack;
    }
}
public enum WeaponType
{
    [InspectorName("メイン")]
    Main,
    [InspectorName("サブ")]
    Sub,
}