using Script.System.Ingame;
using UnityEngine;

public class Bullet_B : MonoBehaviour
{
    Vector3 _moveDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += _moveDirection * 100 * Time.deltaTime;
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
    public virtual void SetDirection(Vector3 dir)
    {
        _moveDirection = dir;
    }
    public virtual void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
