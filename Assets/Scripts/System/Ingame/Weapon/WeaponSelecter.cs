using System.Collections.Generic;
using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    private EquipmentData _playerData;
    public EquipmentData PlayerData => _playerData;

    private MoneyManager _moneyManager;

    // アンロック時に発火するイベント
    public event System.Action OnUnlock;

    private void Awake()
    {
        ServiceLocator.Set(this);
        _playerData = SaveLoadService.Load<EquipmentData>();
    }

    private void Start()
    {
        _moneyManager = ServiceLocator.Get<MoneyManager>();
    }

    public bool SelectWeapon(EquipmentType type, int id)
    {
        bool result = _playerData.EquipWeapon(type, id);

        SaveLoadService.Save(_playerData);

        return result;
    }

    public bool TryBuyWeapon(WeaponData data)
    {
        // お金が足りなかったら買えない
        if (!_moneyManager.CanUseMoney(data.WeaponMoney))
            return false;

        // 購入できたら武器をアンロック
        _moneyManager.UseMoney(data.WeaponMoney);

        _playerData.UnlockWeapon(data.WeaponID);

        OnUnlock?.Invoke();

        return true;
    }

    public List<int> GetUnlockIDs() => _playerData.UnlockedWeaponIds;
}
