using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using SymphonyFrameWork.System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private SceneListEnum _currentScene = SceneListEnum.None;
    
    private void Awake()
    {
        //最初のアクティブシーンをcurrent sceneにする
        var active = SceneManager.GetActiveScene();
        if (!Enum.TryParse(active.name, out _currentScene))
        {
            Debug.LogWarning("Scene '" + active.name + "' is not a valid SceneListEnum");
        }
    }
    
    /// <summary>
    /// 現在のシーンをアンロードし、引数のシーンをロードする
    /// </summary>
    /// <param name="scene"></param>
    public async void ChangeScene(SceneListEnum scene)
    {
        await FadeOut();
        
        //ロードシーンを読み込む
        await SceneLoader.LoadScene(SceneListEnum.Load.ToString());
        
        await FadeIn();
        
        //現在のシーンをアンロード
        await SceneLoader.UnloadScene(_currentScene.ToString());
        
        //次のシーンを読み込み
        await SceneLoader.LoadScene(scene.ToString());
        _currentScene = scene;
        
        await FadeOut();
        
        //ロードシーンをアンロード
        await SceneLoader.UnloadScene(SceneListEnum.Load.ToString());
        
        await FadeIn();
    }

    private async Task FadeIn()
    {
        await Awaitable.WaitForSecondsAsync(0.3f);
    }

    private async Task FadeOut()
    {
        await Awaitable.WaitForSecondsAsync(0.3f);
    }
}
