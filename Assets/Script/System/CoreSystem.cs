using SymphonyFrameWork.CoreSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoreSystem : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeGame()
    {
        _ = SceneLoader.LoadScene(SceneListEnum.System.ToString());
    }
}
