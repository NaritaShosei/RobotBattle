using SymphonyFrameWork.CoreSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoreSystem : MonoBehaviour
{
    /// <summary>
    /// ゲーム開始時にシステムシーンをロード
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeGame()
    {
        _ = SceneLoader.LoadScene(SceneListEnum.System.ToString());
    }
}
