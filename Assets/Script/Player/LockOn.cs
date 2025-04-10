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
            Vector3 viewPos = Camera.main.WorldToViewportPoint(enemy.transform.position);

            if (viewPos.z < lockOnCenterScreenPos.y) continue;

            Vector2 targetScreenPos = new Vector2(viewPos.x, viewPos.y);

            var dis = Vector2.Distance(lockOnCenterScreenPos, targetScreenPos);

            if (dis < minDistance && dis <= 0.5f)
            {
                minDistance = dis;
                _lockOnEnemy = enemy;
            }
        }
    }
    public Enemy_B GetTarget()
    {
        return _lockOnEnemy;
    }
}
