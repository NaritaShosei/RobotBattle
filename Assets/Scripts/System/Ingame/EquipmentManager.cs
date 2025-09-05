using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    private int _mainID;
    private int _subID;

    private void Awake()
    {
        ServiceLocator.Set(this);
        var data = SaveLoadService.Load<EquipmentData>();
        _mainID = data.CurrentLoadout.PrimaryWeaponId;
        _subID = data.CurrentLoadout.SecondWeaponId;
    }

    public void SpawnWeapon(EquipmentType type,Transform parent)
    {
        int id = type switch
        {
            EquipmentType.Main => _mainID,
            EquipmentType.Sub => _subID,
        };

        var weaponData = ServiceLocator.Get<WeaponManager>().DataBase.GetWeapon(id);
        if (weaponData?.WeaponPrefab == null) { Debug.LogWarning("プレハブが設定されていません"); return; }

        var weaponObj = Instantiate(weaponData.WeaponPrefab, parent);
        //var weaponComponent = weaponObj.GetComponent<WeaponBase>();

        //if (!weaponComponent) { Debug.LogWarning("プレハブにWeaponBaseがアタッチされていません"); return; }

        //weaponComponent.Initialize(weaponData);
    }
}
public enum EquipmentType
{
    Main,
    Sub,
}