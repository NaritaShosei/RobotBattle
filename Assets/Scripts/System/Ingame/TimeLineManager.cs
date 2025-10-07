using UnityEngine;
using UnityEngine.Playables;

public class TimeLineManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector _playableDirector;
    public PlayableDirector PlayableDirector => _playableDirector;

    private void Awake()
    {
        ServiceLocator.Set(this);
    }
}
