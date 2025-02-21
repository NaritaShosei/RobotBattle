using System;
using UnityEngine;

/// <summary>
/// ランタイム時に破壊されるデバッグ用オブジェクト
/// </summary>
public class DestroyOnRuntimeObject : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject);
    }
}
