using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        ServiceLocator.Set(this);
    }

    void Start()
    {

    }
}
