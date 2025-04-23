using UnityEngine;

public class SmallEnemy : Enemy_B
{
    [SerializeField]
    void Start()
    {
        OnStart();
        Initialize(_dataBase);
    }

    void Update()
    {
        OnAttackEvent?.Invoke(_player);
    }
}
