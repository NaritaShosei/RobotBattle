using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }

    [SerializeField]
    PlayerManager _player;

    [SerializeField]
    int _maxTime;

    float _time;

    public bool IsTimeOver => _time <= 0;
    private void Awake()
    {
        _time = _maxTime;
        Instance = this;
    }
    void Start()
    {
        //GameUIManagerの初期化がAwakeで行われているのでStartでTimeの初期化
        GameUIManager.Instance.TimeView.SetTime(_time);
    }

    // Update is called once per frame
    void Update()
    {
        //Playerが死んでいたら処理を抜ける
        if (_player.State == PlayerState.Dead)
        {
            return;
        }

        //すでに時間切れの場合処理を抜ける
        if (IsTimeOver)
        {
            return;
        }

        _time = Mathf.Max(_time - Time.deltaTime, 0);
        GameUIManager.Instance.TimeView.SetTime(_time);
        //時間切れになった際の処理
        if (IsTimeOver)
        {

        }
    }
}
