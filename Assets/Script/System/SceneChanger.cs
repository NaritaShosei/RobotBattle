using UnityEngine;
using UnityEngine.SceneManagement;
public static class SceneChanger
{
    public static void LoadScene(SceneType type)
    {
        SceneManager.LoadScene((int)type);
    }
}

public enum SceneType
{
    Title = 0,
    Ingame = 1,
}