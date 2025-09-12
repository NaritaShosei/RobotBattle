using System;
using UnityEngine;

/// <summary>
/// ロックオン可能な対象
/// </summary>
public interface ILockOnTarget
{
    Transform GetTransform();      // 本体の位置
    Transform GetTargetCenter();   // ロックオン用の中心点
    bool IsTargetInView();         // 画面内にいるかどうか
}

/// <summary>
/// 攻撃可能な対象
/// </summary>
public interface IFightable : ILockOnTarget
{
    void HitDamage(Collider other); // 攻撃を受けたとき
}

/// <summary>
/// 通常の敵
/// </summary>
public interface IEnemy : IFightable
{
    void AddOnAttackEvent(Action<PlayerController> action);
    void RemoveOnAttackEvent(Action<PlayerController> action);
}

/// <summary>
/// 敵スポナー
/// </summary>
public interface ISpawner : IFightable
{
    event Action<ISpawner> OnDestroyed;
    event Action<IEnemy> OnEnemySpawned;
}
