using UnityEngine;

[CreateAssetMenu(menuName = "EquipmentData/SpecialData", fileName = "SpecialData")]
public class SpecialData : ScriptableObject,IData
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
    [SerializeField] private float _duration = 1f; // 持続時間
    [SerializeField] private float _requiredGauge; // 発動に必要なゲージ量

    [Header("装備情報")]
    [SerializeField] private GameObject _prefab;
    [SerializeField] private EquipmentType _equipmentType;

    public int ID => _id;
    public string Name => _name;
    public Sprite Icon => _icon;
    public int Money => _money;
    public int Cost => _cost;

    public float AttackPower => _attackPower;
    public float Range => _range;
    public float GuardBreak => _guardBreak;
    public float Duration => _duration;
    public float RequiredGauge => _requiredGauge;

    public GameObject Prefab => _prefab;
    public EquipmentType EquipmentType => _equipmentType;
}
