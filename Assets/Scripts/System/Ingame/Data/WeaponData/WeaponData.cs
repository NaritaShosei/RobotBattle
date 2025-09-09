using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData", fileName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("基本情報")]
    [SerializeField] private int _weaponID;
    [SerializeField] private string _weaponName;
    [SerializeField] private Sprite _weaponIcon;
    [SerializeField] private int _weaponMoney;
    [SerializeField] private int _weaponCost;

    [Header("性能")]
    [SerializeField] private float _attackPower = 1;
    [SerializeField] private float _attackRate = 1;
    [SerializeField] private float _range = 10;
    [SerializeField] private float _coolTime = 1.5f;
    [SerializeField] private int _capacity = 10; // 装弾数
    [SerializeField] private float _guardBreak = 50;
    [SerializeField] private float _speed = 100;

    [Header("プレハブ")]
    [SerializeField] private GameObject _weaponPrefab;
    public int WeaponID => _weaponID;
    public string WeaponName => _weaponName;
    public Sprite WeaponIcon => _weaponIcon;
    public int WeaponMoney => _weaponMoney;
    public int WeaponCost => _weaponCost;
    public float AttackPower => _attackPower;
    public float AttackRate => _attackRate;
    public float Range => _range;
    public float CoolTime => _coolTime;
    public int AttackCapacity => _capacity;
    public float AttackSpeed => _speed;
    public float GuardBreakValue => _guardBreak;
    public GameObject WeaponPrefab => _weaponPrefab;

}
