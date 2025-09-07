using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private WeaponDatabase _dataBase;

    public WeaponDatabase DataBase => _dataBase;

    private void Awake()
    {
        ServiceLocator.Set(this);
    }
}
