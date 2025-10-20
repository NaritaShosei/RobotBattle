using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponSelectView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private WeaponType _type;
    private EquipmentDatabase _weaponDatabase;
    [SerializeField] private WeaponCell _weaponCell;
    [SerializeField] private Transform _cellParent;
    [SerializeField] private WeaponExplanation _explanation;
    private WeaponCell _currentCell;
    private List<WeaponCell> _cells = new();
    [Header("アニメーション設定")]
    [SerializeField] private float _animationDuration = 0.2f;
    private WeaponSelector _selector;

    private void Start()
    {
        _weaponDatabase = ServiceLocator.Get<WeaponManager>().DataBase;
        _selector = ServiceLocator.Get<WeaponSelector>();
        SetUI();
        _selector.OnUnlock += ResetUI;
    }

    private void SetUI()
    {
        foreach (var id in _selector.GetUnlockIDs())
        {
            var data = _weaponDatabase.GetWeapon(id);

            var cell = Instantiate(_weaponCell, _cellParent);
            cell.Initialize(data.WeaponIcon, data.WeaponName, "cost", data.WeaponCost, data);
            _cells.Add(cell);
        }

        int equippedID = _type switch
        {
            WeaponType.Main => _selector.PlayerData.CurrentLoadout.PrimaryWeaponId,
            WeaponType.Sub => _selector.PlayerData.CurrentLoadout.SecondWeaponId,
        };

        _currentCell = _cells.FirstOrDefault(cell => cell.WeaponData.ID == equippedID);

        if (_currentCell != null)
        {
            _currentCell.Select();
            SetExplanation(_currentCell.WeaponData.ID);
        }
    }
    private void ResetUI()
    {
        foreach (var cell in _cells)
            Destroy(cell.gameObject);

        _cells.Clear();

        SetUI();
    }

    private void SetExplanation(int id)
    {
        var data = _weaponDatabase.GetWeapon(id);

        if (data != null)
            _explanation.Set(data);
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

            _selector.SelectWeapon(_type, _currentCell.WeaponData.ID);
            SetExplanation(_currentCell.WeaponData.ID);
        }
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
