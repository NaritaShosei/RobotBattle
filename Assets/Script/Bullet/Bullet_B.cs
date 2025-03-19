using Script.System.Ingame;
using UnityEngine;

public class Bullet_B : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (TryGetComponent(out IFightable component))
        {
            AddDamage(0, component);
        }
    }

    protected virtual void AddDamage(float damage, IFightable fightable)
    {

    }
}
