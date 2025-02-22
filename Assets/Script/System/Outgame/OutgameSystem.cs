using SymphonyFrameWork.System;
using UnityEngine;

public class OutgameSystem : MonoBehaviour
{
    /// <summary>
    /// インゲームをロードする
    /// </summary>
    [ContextMenu("StartIngame")]
    public void StartIngame()
    {
        var changer = ServiceLocator.GetInstance<SceneChanger>();
        changer.ChangeScene(SceneListEnum.Ingame);
    }
}
