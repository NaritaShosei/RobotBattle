using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private BasicButton _button;
    private void Start()
    {
        ServiceLocator.Get<FadePanel>().Fade(0).Forget();
        _button.OnClick += OnClick;
    }

    private async void OnClick()
    {
        await ServiceLocator.Get<FadePanel>().Fade(1);
        SceneChanger.LoadScene(SceneChanger.INGAME);
    }
}
