using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Enemy")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Enemy", message: "EventChannel", category: "Events", id: "2fabd623c165bf1b161dcf34fe37907e")]
public partial class Enemy : EventChannelBase
{
    public delegate void EnemyEventHandler();
    public event EnemyEventHandler Event; 

    public void SendEventMessage()
    {
        Event?.Invoke();
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        Event?.Invoke();
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        EnemyEventHandler del = () =>
        {
            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as EnemyEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as EnemyEventHandler;
    }
}

