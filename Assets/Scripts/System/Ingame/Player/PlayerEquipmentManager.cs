using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    [SerializeField] private Transform _left;
    [SerializeField] private Transform _right;
    [SerializeField] private Transform _center;

    private Dictionary<EquipmentType, Transform> _parents;

    private void Awake()
    {
        _parents = new Dictionary<EquipmentType, Transform>
        {
            { EquipmentType.Left, _left },
            { EquipmentType.Right, _right },
            { EquipmentType.Center, _center }
        };

        ServiceLocator.Set(this);
    }

    public Transform GetEquipmentParent(EquipmentType type)
    {
        return _parents[type];
    }
}

public enum EquipmentType
{
    Left,
    Right,
    Center,
}
