using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LockOn : MonoBehaviour
{
    [SerializeField]
    Transform _player;

    [SerializeField, Header("0～1の間 Debug Only")]
    Vector2 _lockOnCenterScreenPos;

    [SerializeField]
    float _maxDistance;

    [SerializeField, Range(0, 180)]
    float _viewAngleHorizontal = 120;

    [SerializeField, Range(0, 180)]
    float _viewAngleVertical = 60;


    Enemy_B _lockOnEnemy;
    List<Enemy_B> _enemies;
    void Start()
    {
        _enemies = FindObjectsByType<Enemy_B>(FindObjectsSortMode.None).ToList();
    }

    void Update()
    {

        float minDistance = float.MaxValue;
        _lockOnEnemy = null;

        foreach (var enemy in _enemies)
        {
            Vector3 enemyDir = enemy.transform.position - _player.position;

            var toEnemyDir = enemyDir.normalized;
            var forward = Camera.main.transform.forward.normalized;

            enemyDir.y = 0;
            if (enemyDir.magnitude > _maxDistance) continue;

            Vector3 viewPos = Camera.main.WorldToViewportPoint(enemy.transform.position);
            if (viewPos.z < 0f) continue;

            Vector3 enemyDirFlat = new Vector3(toEnemyDir.x, 0, toEnemyDir.z).normalized;
            Vector3 flatForward = new Vector3(forward.x, 0, forward.z).normalized;

            float dotHorizontal = Vector3.Dot(flatForward, enemyDirFlat);

            if (dotHorizontal < Mathf.Cos(_viewAngleHorizontal * 0.5f * Mathf.Deg2Rad)) continue;

            float verticalDis = new Vector3(toEnemyDir.x, 0, toEnemyDir.z).magnitude;
            Vector3 verticalEnemy = new Vector3(0, toEnemyDir.y, verticalDis).normalized;

            float forwardFlatDis = new Vector3(forward.x, 0, forward.z).magnitude;
            var verticalForward = new Vector3(0, forward.y, forwardFlatDis).normalized;

            float dotVertical = Vector3.Dot(verticalForward, verticalEnemy);
            if (dotVertical < Mathf.Cos(_viewAngleVertical * 0.5f * Mathf.Deg2Rad)) continue;

            Vector2 targetScreenPos = new Vector2(viewPos.x, viewPos.y);
            float screenDis = Vector2.Distance(_lockOnCenterScreenPos, targetScreenPos);

            enemyDir.y = 0;

            if (screenDis < minDistance)
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
