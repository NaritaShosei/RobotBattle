using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [SerializeField] TimeView _timeView;
    public TimeView TimeView => _timeView;

    private void Awake()
    {
        Instance = this;
    }
}
