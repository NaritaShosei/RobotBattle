using UnityEngine;

public class SystemSceneManager : MonoBehaviour
{
    static bool _isInitialize = false;

    private void Awake()
    {
        if (_isInitialize)
        {
            Destroy(gameObject);
            return;
        }
        _isInitialize = true;
        DontDestroyOnLoad(gameObject);
    }
}
