using UnityEngine;

public class BossEnemy : MovingEnemy
{
    private bool _isAlive = true;
    public bool IsAlive => _isAlive;
    protected override void Dead()
    {
        base.Dead();
        _isAlive = false;
    }
}
