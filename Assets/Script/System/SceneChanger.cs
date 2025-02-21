using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using SymphonyFrameWork.System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneChanger : MonoBehaviour
{
    private SceneListEnum _currentScene = SceneListEnum.None;

    private VisualElement _fadeBackGround;
    
    private void Awake()
    {
        //最初のアクティブシーンをcurrent sceneにする
        var active = SceneManager.GetActiveScene();
        if (!Enum.TryParse(active.name, out _currentScene))
        {
            Debug.LogWarning("Scene '" + active.name + "' is not a valid SceneListEnum");
        }
        
        //フェード演出用の黒背景を取得
        var document = GetComponentInChildren<UIDocument>();
        _fadeBackGround = document.rootVisualElement;
    }

    private void Start()
    {
        _fadeBackGround.style.opacity = 0;
    }

    /// <summary>
    /// 現在のシーンをアンロードし、引数のシーンをロードする
    /// </summary>
    /// <param name="scene"></param>
    public async void ChangeScene(SceneListEnum scene)
    {
        await FadeOut(1f);
        
        //ロードシーンを読み込む
        await SceneLoader.LoadScene(SceneListEnum.Load.ToString());
        SceneLoader.SetActiveScene(SceneListEnum.Load.ToString());
        
        await FadeIn(1f);
        
        //現在のシーンをアンロード
        await SceneLoader.UnloadScene(_currentScene.ToString());
        
        //次のシーンを読み込み
        await SceneLoader.LoadScene(scene.ToString());
        _currentScene = scene;
        
        await FadeOut(1f);
        
        SceneLoader.SetActiveScene(scene.ToString());
        
        //ロードシーンをアンロード
        await SceneLoader.UnloadScene(SceneListEnum.Load.ToString());
        
        await FadeIn(1f);
    }

    private async Task FadeIn(float timer)
    {
        float speed = 1 / timer; //透明度の秒間変化スピード
        
        do
        {
            //透明度を変化させる
            _fadeBackGround.style.opacity = _fadeBackGround.style.opacity.value
                                            - speed * Time.deltaTime;
            
            await Awaitable.NextFrameAsync();
            timer -= Time.deltaTime;
        } while (timer > 0);

        _fadeBackGround.style.opacity = 0;
    }

    private async Task FadeOut(float timer)
    {
        float speed = 1 / timer; //透明度の秒間変化スピード
        
        do
        {
            //透明度を変化させる
            _fadeBackGround.style.opacity = _fadeBackGround.style.opacity.value
                                            + speed * Time.deltaTime;
            
            await Awaitable.NextFrameAsync();
            timer -= Time.deltaTime;
        } while (timer > 0);
        
        _fadeBackGround.style.opacity = 1;
    }
}
