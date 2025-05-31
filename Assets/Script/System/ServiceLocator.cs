using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    public static Dictionary<Type, object> _service = new();

    public static void Set<T>(T service)
    {
        if (_service.ContainsKey(typeof(T)))
        {
            _service[typeof(T)] = service;
            return;
        }
        _service.Add(typeof(T), service);
    }

    public static T Get<T>()
    {
        return (T)_service[typeof(T)];
    }
}
