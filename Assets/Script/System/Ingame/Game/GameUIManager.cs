using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [SerializeField] TimeView _timeView;
    public TimeView TimeView => _timeView;

    [SerializeField] WeaponView _weaponView;
    public WeaponView WeaponView => _weaponView;

    [SerializeField] HPGaugeView _hpgaugeView;
    public HPGaugeView HPGaugeView => _hpgaugeView;

    private void Awake()
    {
        Instance = this;
    }
}
