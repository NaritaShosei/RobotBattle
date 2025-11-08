using UnityEngine;

[DefaultExecutionOrder(-100)]
public class WeaponManager : MonoBehaviour
{
    [SerializeField] private EquipmentDatabase _dataBase;

    public EquipmentDatabase DataBase => _dataBase;

    private void Awake()
    {
        ServiceLocator.Set(this);
    }
}
