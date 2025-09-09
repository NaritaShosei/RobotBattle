using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private BasicButton _button;
    [SerializeField] private TMP_Text _moneyText;
    private void Start()
    {
        ServiceLocator.Get<FadePanel>().Fade(0).Forget();
        _button.OnClick += OnClick;
        SetMoney();
        ServiceLocator.Get<WeaponSelector>().OnUnlock += SetMoney;
    }

    private async void OnClick()
    {
        await ServiceLocator.Get<FadePanel>().Fade(1);
        SceneChanger.LoadScene(SceneChanger.INGAME);
    }

    private void SetMoney()
    {
        _moneyText.text = $"${ServiceLocator.Get<MoneyManager>().GetMoney():0000000}";
    }
}
