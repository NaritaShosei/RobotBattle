using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameResultView : MonoBehaviour
{
    [SerializeField] GameObject _panel;

    [SerializeField] Text _resultText;

    [SerializeField] Text _scoreText;

    [SerializeField] Text _gameOverTypeText;

    [SerializeField] Text _timeText;

    [SerializeField] private BasicButton _button;

    private void Start()
    {
        _button.OnClick += OnClick;
        HideUI();
    }

    private async void OnClick()
    {
        await ServiceLocator.Get<FadePanel>().Fade(1);
        SceneChanger.LoadScene(SceneChanger.TITLE);
    }

    public void ShowUI()
    {
        _panel.SetActive(true);
    }

    public void HideUI()
    {
        _panel.SetActive(false);
    }


    public void SetGameClear()
    {
        _resultText.text = "GameClear";
    }

    public void SetGameOverType(GameOverType type)
    {
        _timeText.enabled = false;
        _gameOverTypeText.enabled = true;
        _resultText.text = "GameOver";
        _gameOverTypeText.text = type switch
        {
            GameOverType.TimeOver => "TimeOver",
            GameOverType.Death => "PlayerDead",
            _ => ""
        };
    }
    public void SetScore(int score)
    {
        _scoreText.text = score.ToString();
    }
    public void SetTime(float time)
    {
        _timeText.enabled = true;
        _gameOverTypeText.enabled = false;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        _timeText.text = $"{minutes:00}:{seconds:00}";
    }
}
