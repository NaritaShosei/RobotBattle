using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    /// <summary>
    /// 弾を管理するプール
    /// </summary>
    Dictionary<LongRangeAttack_B, Queue<Bullet_B>> _pools = new();
    private void Awake()
    {
        ServiceLocator.Set(this);
    }

    /// <summary>
    /// 対応するプールを作成する
    /// </summary>
    /// <param name="key"></param>
    /// <param name="bullet">弾</param>
    /// <param name="count">最大数</param>
    public Bullet_B[] SetPool(LongRangeAttack_B key, Bullet_B bullet, int count, LayerMask layer)
    {
        if (!_pools.ContainsKey(key))
        {
            var pool = new Queue<Bullet_B>();

            GameObject parent = new GameObject($"{key.gameObject.name}Pool");

            parent.transform.SetParent(transform);
            //countの分作成
            for (int i = 0; i < count; i++)
            {
                var b = Instantiate(bullet, parent.transform);
                b.ReturnPoolEvent = () => ReturnPool(key, b);
                b.gameObject.SetActive(false);
                b.gameObject.layer = Mathf.RoundToInt(Mathf.Log(layer.value, 2));
                pool.Enqueue(b);
            }

            _pools.Add(key, pool);
        }
        return _pools[key].ToArray();
    }

    /// <summary>
    /// 弾を取り出す
    /// </summary>
    /// <param name="key"></param>
    /// <returns弾を返す。プールが空だったらnullを返す。</returns>
    public Bullet_B GetBullet(LongRangeAttack_B key)
    {
        if (_pools.TryGetValue(key, out Queue<Bullet_B> pool))
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
        }

        return null;
    }

    /// <summary>
    /// プールに弾を戻す
    /// </summary>
    /// <param name="key"></param>
    /// <param name="bullet"></param>
    public void ReturnPool(LongRangeAttack_B key, Bullet_B bullet)
    {
        if (_pools.ContainsKey(key))
        {
            _pools[key].Enqueue(bullet);
        }
    }

    public bool IsPoolCount(LongRangeAttack_B key)
    {
        if (_pools.TryGetValue(key, out Queue<Bullet_B> pool))
        {
            return pool.Count > 0;
        }
        return false;
    }
}
