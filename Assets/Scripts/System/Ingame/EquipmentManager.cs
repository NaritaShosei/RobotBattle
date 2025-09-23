using Unity.VisualScripting;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    private int _mainID;
    private int _subID;
    private int _specialID;
    PlayerEquipmentManager _playerEquipmentManager;

    private void Awake()
    {
        ServiceLocator.Set(this);
        var data = SaveLoadService.Load<EquipmentData>();
        _mainID = data.CurrentLoadout.PrimaryWeaponId;
        _subID = data.CurrentLoadout.SecondWeaponId;
    }

    private void Start()
    {
        _playerEquipmentManager = ServiceLocator.Get<PlayerEquipmentManager>();
    }

    public WeaponBase SpawnWeapon(WeaponType type)
    {
        int id = type switch
        {
            WeaponType.Main => _mainID,
            WeaponType.Sub => _subID,
        };

        var weaponData = ServiceLocator.Get<WeaponManager>().DataBase.GetWeapon(id);
        if (weaponData?.WeaponPrefab == null) { Debug.LogWarning("プレハブが設定されていません"); return null; }

        var parent = _playerEquipmentManager.GetEquipmentParent(weaponData.EquipmentType);

        var weaponObj = Instantiate(weaponData.WeaponPrefab, parent);

        if (!weaponObj.TryGetComponent(out WeaponBase weaponComponent))
        {
            Debug.LogWarning("プレハブにWeaponBaseがアタッチされていません"); return null;
        }

        weaponComponent.Initialize(weaponData);

        return weaponComponent;
    }

    public SpecialAttackBase SpawnSpecial()
    {
        // データベースからIDで検索できる機能ができたら生成処理
        return null;
    }
}
public enum WeaponType
{
    [InspectorName("メイン")]
    Main,
    [InspectorName("サブ")]
    Sub,
}