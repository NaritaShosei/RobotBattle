using UnityEngine;
using UnityEngine.UI;

public class GameResultView : MonoBehaviour
{
    [SerializeField] GameObject _slot;

    [SerializeField] Text _resultText;

    [SerializeField] Text _scoreText;

    [SerializeField] Text _gameOverTypeText;

    [SerializeField] Text _timeText;

    public void SetGameType(ResultType type)
    {
        _resultText.text = type.ToString();
    }

    public void SetGameOverType(GameOverType type)
    {
        _gameOverTypeText.text = type switch
        {
            GameOverType.TimeOver => "TimeOver",
            GameOverType.Death => "PlayerDead",
            _ => ""
        };
    }
}
