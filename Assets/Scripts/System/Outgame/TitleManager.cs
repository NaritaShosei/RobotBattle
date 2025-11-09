using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private BasicButton _button;
    [SerializeField] private TMP_Text _moneyText;
    private void Start()
    {
        // スタート時なので1→0のフェード
        ServiceLocator.Get<FadePanel>().Fade(1, 0).Forget();
        _button.OnClick += OnClick;
        SetMoney();
        ServiceLocator.Get<WeaponSelector>().OnUnlock += SetMoney;
    }

    private async void OnClick()
    {
        // 終了時なので0→1のフェード
        await ServiceLocator.Get<FadePanel>().Fade(0, 1);
        SceneChanger.LoadScene(SceneChanger.INGAME);
    }

    private void SetMoney()
    {
        _moneyText.text = $"${ServiceLocator.Get<MoneyManager>().GetMoney():0000000}";
    }
}
