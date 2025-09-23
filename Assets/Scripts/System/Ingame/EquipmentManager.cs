using Unity.VisualScripting;
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

    public WeaponBase SpawnWeapon(WeaponType type, Transform parent)
    {
        int id = type switch
        {
            WeaponType.Main => _mainID,
            WeaponType.Sub => _subID,
        };

        var weaponData = ServiceLocator.Get<WeaponManager>().DataBase.GetWeapon(id);
        if (weaponData?.WeaponPrefab == null) { Debug.LogWarning("プレハブが設定されていません"); return null; }

        var weaponObj = Instantiate(weaponData.WeaponPrefab, parent);

        if (!weaponObj.TryGetComponent(out WeaponBase weaponComponent))
        {
            Debug.LogWarning("プレハブにWeaponBaseがアタッチされていません"); return null;
        }

        weaponComponent.Initialize(weaponData);

        return weaponComponent;
    }
}
public enum WeaponType
{
    [InspectorName("メイン")]
    Main,
    [InspectorName("サブ")]
    Sub,
}