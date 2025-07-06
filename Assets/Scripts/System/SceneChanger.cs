using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChanger : MonoBehaviour
{
    public const int TITLE = 0;
    public const int INGAME = 1;
    private void Awake()
    {
        ServiceLocator.Set(this);
    }

    public static void LoadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
}