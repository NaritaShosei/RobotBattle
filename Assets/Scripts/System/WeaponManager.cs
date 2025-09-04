using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponManager : MonoBehaviour, IPointerClickHandler
{
    public LongRangeAttack_B MainWeapon { get; private set; }
    public LongRangeAttack_B SubWeapon { get; private set; } 
    private void Awake()
    {
        ServiceLocator.Set(this);
    }

    public void SetMainWeapon(WeaponCellView view)
    {
        //MainWeapon = view.Weapon;
    }
    public void SetSubWeapon(WeaponCellView view)
    {
        //SubWeapon = view.Weapon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var obj = eventData.pointerCurrentRaycast.gameObject;

        if (obj.TryGetComponent(out WeaponCellView view))
        {

        }
    }
}
