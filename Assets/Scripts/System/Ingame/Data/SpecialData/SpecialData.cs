using UnityEngine;

[CreateAssetMenu(menuName = "EquipmentData/SpecialData", fileName = "SpecialData")]
public class SpecialData : ScriptableObject
{
    [Header("基本情報")]
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;
    [SerializeField] private int _money;
    [SerializeField] private int _cost;

    [Header("性能")]
    [SerializeField] private float _attackPower = 1f;
    [SerializeField] private float _range = 10f;
    [SerializeField] private float _guardBreak = 50f;

    [Header("プレハブ")]
    [SerializeField] private GameObject _prefab;

    public int ID => _id;
    public string Name => _name;
    public Sprite Icon => _icon;
    public int Money => _money;
    public int Cost => _cost;

    public float AttackPower => _attackPower;
    public float Range => _range;
    public float GuardBreak => _guardBreak;

    public GameObject Prefab => _prefab;
}
