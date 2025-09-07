using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour
{
    [SerializeField]
    Text _scoreText;

    public void SetScore(int score)
    {
        _scoreText.text = $"{score:000000}";
    }
}
