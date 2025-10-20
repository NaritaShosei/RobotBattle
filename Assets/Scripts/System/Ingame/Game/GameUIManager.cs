using UnityEngine;

public class GameUIManager : MonoBehaviour
{
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

    [SerializeField] CrosshairView _crosshairView;
    public CrosshairView CrosshairView => _crosshairView;

    [SerializeField] SpecialGaugeView _specialGaugeView;
    public SpecialGaugeView SpecialGaugeView => _specialGaugeView;

    private void Awake()
    {
        ServiceLocator.Set(this);
    }
}
