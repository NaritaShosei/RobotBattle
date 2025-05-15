using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [SerializeField] TimeView _timeView;
    public TimeView TimeView => _timeView;

    [SerializeField] WeaponView _weaponView;
    public WeaponView WeaponView => _weaponView;

    [SerializeField] HPGaugeView _hpgaugeView;
    public HPGaugeView HPGaugeView => _hpgaugeView;

    [SerializeField] GaugeView _gaugeView;
    public GaugeView GaugeView => _gaugeView;

    [SerializeField] ScoreView _scoreView;
    public ScoreView ScoreView => _scoreView;

    [SerializeField] GameResultView _gameResultView;
    public GameResultView GameResultView => _gameResultView;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }
}
