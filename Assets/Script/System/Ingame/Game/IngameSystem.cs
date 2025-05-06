using SymphonyFrameWork.System;
using UnityEngine;

public class IngameSystem : MonoBehaviour
{
    /// <summary>
    /// アウトゲームをロードする
    /// </summary>
    [ContextMenu("EndIngame")]
    public void EndIngame()
    {
        var changer = ServiceLocator.GetInstance<SceneChanger>();
        changer.ChangeScene(SceneListEnum.Outgame);
    }
}
