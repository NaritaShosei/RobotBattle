using Script.System.Ingame;
using UnityEngine;

public class SmallEnemy : Enemy_B<CharacterData_B>
{
    [SerializeField] CharacterData_B _dataBase;
    [SerializeField] SmallEnemyTargetZone _collider;
    [SerializeField] Transform _muzzleModel;

    void Start()
    {
        OnStart();
        Initialize(_dataBase);
        _collider.OnTriggerEnterEvent += TargetSet;
        _collider.OnTriggerExitEvent += TargetUnset;
    }

    void Update()
    {
        if (_player)
        {
            _muzzleModel.forward = _player.GetTargetCenter().position - _muzzleModel.transform.position;
            OnAttackEvent?.Invoke(_player);
        }
    }

    public override void TargetSet(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _player = player;
        }
    }

    public override void TargetUnset(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _player = null;
        }
    }
}
