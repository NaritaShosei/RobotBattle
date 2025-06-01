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
        if (_gameManager.IsPaused) { return; }
        if (_player)
        {
            _muzzleModel.forward = _player.GetTargetCenter().position - _muzzleModel.transform.position;
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
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError("SmallEnemy" + _data.Health);
    }
}
