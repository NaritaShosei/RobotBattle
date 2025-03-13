using System.Collections.Generic;
using UnityEngine;

public class LockOn : MonoBehaviour
{
    List<(GameObject obj, Enemy_B enemy)> _enemyList = new();
    [SerializeField]
    float _sightAngle;
    [SerializeField]
    float _maxDistance;
    void Start()
    {

    }

    void Update()
    {

    }
}
