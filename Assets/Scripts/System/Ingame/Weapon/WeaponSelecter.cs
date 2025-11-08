using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class WeaponSelector : MonoBehaviour
{
    private EquipmentData _playerData;
    public EquipmentData PlayerData => _playerData;

    private MoneyManager _moneyManager;
    private EquipmentDatabase _database;

    public event Action OnUnlock;

    public event Action OnEquipComplete;

    private async void Awake()
    {
        ServiceLocator.Set(this);

        // データロード
        _playerData = SaveLoadService.Load<EquipmentData>();

        try
        {
            await UniTask.WaitUntil(() => ServiceLocator.Get<WeaponManager>(), cancellationToken: destroyCancellationToken);
        }
        catch { }

        // Databaseを取得（WeaponManagerが先に初期化されている前提）
        _database = ServiceLocator.Get<WeaponManager>().DataBase;

        // 初回起動時の初期化
        ValidateInitialSetup();
    }

    private void Start()
    {
        _moneyManager = ServiceLocator.Get<MoneyManager>();
    }

    /// <summary>
    /// 初期装備が未設定なら自動で付与
    /// </summary>
    private void ValidateInitialSetup()
    {
        if (_playerData.UnlockedWeaponIds.Count == 0)
        {
            Debug.Log("初回起動: 初期装備を付与");

            var initialWeapons = _database.GetInitialWeaponIds();
            var initialSpecials = _database.GetInitialSpecialIds();

            _playerData.ApplyInitialUnlocks(initialWeapons, initialSpecials);

            // 初期装備をセット
            if (initialWeapons.Length > 0)
            {
                _playerData.EquipWeapon(WeaponType.Main, initialWeapons[0]);

                if (initialWeapons.Length > 1)
                    _playerData.EquipWeapon(WeaponType.Sub, initialWeapons[1]);
            }

            if (initialSpecials.Length > 0)
                _playerData.EquipSpecial(initialSpecials[0]);

            SaveLoadService.Save(_playerData);
        }

        // 
        OnEquipComplete?.Invoke();
    }

    public bool SelectWeapon(WeaponType type, int id)
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

        _playerData.UnlockWeapon(data.ID);

        OnUnlock?.Invoke();

        SaveLoadService.Save(_playerData);

        return true;
    }

    public List<int> GetUnlockIDs() => _playerData.UnlockedWeaponIds;
}
