using Script.System.Ingame;
using UnityEngine;

public class ChaseBullet : Bullet_B
{
    [SerializeField] private float _minDistance = 10f;
    [SerializeField] private float _rotateSpeed = 10f; // 回転の追従速度
    [SerializeField] private float _chaseDuration = 1.5f; // 追尾継続時間

    private float _chaseTimer;
    private enum ChaseState { Chasing, Stopped }
    private ChaseState _state;

    protected override void OnEnable_B()
    {
        base.OnEnable_B();
        _state = ChaseState.Chasing;
        _chaseTimer = 0f;
    }

    protected override void OnUpdate()
    {
        _timer += Time.deltaTime;
        _chaseTimer += Time.deltaTime;

        if (_state == ChaseState.Chasing)
        {
            UpdateChase();
        }

        // 弾を前進
        transform.position += transform.forward * _weaponData.AttackSpeed * Time.deltaTime;

        // 時間で消滅
        if (_timer >= _enableTime)
        {
            _isTimeReturned = true;
            gameObject.SetActive(false);
        }
    }

    private void UpdateChase()
    {
        if (_target == null) return;

        var targetPos = _target.GetTargetCenter().position;
        var dir = targetPos - transform.position;
        var sqrDist = dir.sqrMagnitude;

        // 一定距離以内 or 時間経過で追尾終了
        if (sqrDist < _minDistance * _minDistance || _chaseTimer >= _chaseDuration)
        {
            _state = ChaseState.Stopped;
            return;
        }

        // 滑らかに回転
        var targetRot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotateSpeed * Time.deltaTime);
    }
}
