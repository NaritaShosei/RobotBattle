using Script.System.Ingame;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy_B : Character_B<CharacterData_B>
{
    Rigidbody _rb;
    PlayerController _player;
    [SerializeField] CharacterData_B _dataBase;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize(_dataBase);
        _rb = GetComponent<Rigidbody>();
        _player = FindAnyObjectByType<PlayerController>();
    }

    private void Update()
    {
        Move(_player.transform.position);
    }

    void Move(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.Normalize();
        dir.y = 0;
        _rb.linearVelocity = dir * _data.NormalSpeed;
    }




    /// <summary>
    /// とりあえずのデバッグ用
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(_data.Health);
    }
}
