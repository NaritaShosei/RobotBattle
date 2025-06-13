using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    float _duration = 0.5f;
    private void Awake()
    {
        ServiceLocator.Set(this);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //FadeIn
        ServiceLocator.Get<GameUIManager>().PanelUIView.Fade(TargetType.Image, 0, _duration);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
