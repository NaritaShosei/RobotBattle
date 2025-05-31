using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public static Dictionary<Type, object> _service = new();

    public static void Set<T>(T service)
    {
        _service.Add(typeof(T), service);
    }

    public static T Get<T>()
    {
        return (T)_service[typeof(T)];
    }
}
