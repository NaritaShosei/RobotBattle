using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    private void Start()
    {
        ServiceLocator.Get<FadePanel>().Fade(0).Forget();
    }
}
