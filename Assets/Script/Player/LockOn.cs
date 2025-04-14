using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LockOn : MonoBehaviour
{
    [SerializeField]
    Transform _player;

    [SerializeField]
    Transform _lockOnCenter;

    [SerializeField]
    float _maxDistance;

    [SerializeField, Range(0, 180)]
    float _viewAngle = 120;


    Enemy_B _lockOnEnemy;
    List<Enemy_B> _enemies;
    void Start()
    {
        _enemies = FindObjectsByType<Enemy_B>(FindObjectsSortMode.None).ToList();
    }

    void Update()
    {
        Vector3 lockOnCenterScreenPos = Camera.main.WorldToViewportPoint(_lockOnCenter.position);
        float minDistance = float.MaxValue;
        _lockOnEnemy = null;

        foreach (var enemy in _enemies)
        {
            Vector3 enemyDir = enemy.transform.position - _player.position;
            Vector3 enemyDirFlat = new Vector3(enemyDir.x, 0, enemyDir.z).normalized;
            Vector3 playerForward = new Vector3(_player.forward.x, 0, _player.forward.z).normalized;

            float dot = Vector3.Dot(playerForward, enemyDirFlat);
            float cosViewAngle = Mathf.Cos(_viewAngle * 0.5f * Mathf.Deg2Rad);

            if (dot < cosViewAngle) continue;

            Vector3 viewPos = Camera.main.WorldToViewportPoint(enemy.transform.position);
            if (viewPos.z < 0f) continue;

            Vector2 targetScreenPos = new Vector2(viewPos.x, viewPos.y);
            float screenDis = Vector2.Distance(lockOnCenterScreenPos, targetScreenPos);


            enemyDir.y = 0;

            if (screenDis < minDistance && screenDis <= 0.5f && enemyDir.magnitude <= _maxDistance)
            {
                minDistance = screenDis;
                _lockOnEnemy = enemy;
            }
        }
    }
    public Enemy_B GetTarget()
    {
        return _lockOnEnemy;
    }
}
