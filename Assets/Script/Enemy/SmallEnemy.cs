using UnityEngine;

public class SmallEnemy : Enemy_B
{
    [SerializeField] SmallEnemyTargetZone _collider;

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
            OnAttackEvent?.Invoke(_player);
        }
    }

    void TargetSet(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _player = player;
        }
    }

    void TargetUnset(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _player = null;
        }
    }
}
