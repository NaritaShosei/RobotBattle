using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponShopView : MonoBehaviour, IPointerClickHandler
{
    private WeaponDatabase _weaponDatabase;
    [SerializeField] private WeaponCell _weaponCell;
    [SerializeField] private Transform _cellParent;
    [SerializeField] private WeaponExplanation _explanation;
    [SerializeField] private GameObject _failedPopupPanel;
    private WeaponCell _currentCell;
    private List<WeaponCell> _cells = new();
    [Header("アニメーション設定")]
    [SerializeField] private float _animationDuration = 0.2f;
    private WeaponSelector _selector;

    private void Start()
    {
        _failedPopupPanel.SetActive(false);
        _weaponDatabase = ServiceLocator.Get<WeaponManager>().DataBase;
        _selector = ServiceLocator.Get<WeaponSelector>();
        SetUI();
    }

    private void SetUI()
    {
        int[] unlockedIds = _selector.GetUnlockIDs().ToArray();
        int[] allIds = _weaponDatabase.GetAllWeapons().Select(d => d.WeaponID).ToArray();

        int[] lockedIds = allIds.Except(unlockedIds).ToArray();

        List<WeaponData> weapons = new();

        foreach (var id in lockedIds)
        {
            weapons.Add(_weaponDatabase.GetWeapon(id));
        }

        foreach (var data in weapons.OrderBy(d => d.WeaponMoney))
        {
            var cell = Instantiate(_weaponCell, _cellParent);
            cell.Initialize(data.WeaponIcon, data.WeaponName, "$", data.WeaponMoney, data);
            _cells.Add(cell);
        }

        _currentCell = _cells[0];
        _currentCell.Select();
        SetExplanation(_currentCell.WeaponData.WeaponID);
    }

    private void ResetUI()
    {
        foreach (var cell in _cells)
            Destroy(cell.gameObject);

        _cells.Clear();
    }

    private void SetExplanation(int id)
    {
        var data = _weaponDatabase.GetWeapon(id);

        if (data != null)
            _explanation.Set(data.WeaponName, data.AttackPower, data.AttackRate);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
        if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out WeaponCell cell))
        {
            Debug.Log(cell.gameObject.name);

            _currentCell.UnSelect();

            _currentCell = cell;
            _currentCell.Select();

            SetExplanation(_currentCell.WeaponData.WeaponID);
        }
    }

    public void BuyWeapon()
    {
        if (!_selector.TryBuyWeapon(_currentCell.WeaponData))
        {
            Debug.Log("購入できない");
            _failedPopupPanel.SetActive(true);
            return;
        }

        // 購入した武器を削除
        _cells.Remove(_currentCell);
        Destroy(_currentCell.gameObject);

        ResetUI();
        SetUI();
        Debug.Log("購入できた");
    }

    /// <summary>
    /// ボタンクリック時のアニメーション
    /// </summary>
    public void OnClick(int target)
    {
        transform.DOScaleY(target, _animationDuration).
            SetEase(Ease.OutElastic);
    }
}
