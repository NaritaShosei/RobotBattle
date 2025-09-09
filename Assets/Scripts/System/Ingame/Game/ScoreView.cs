using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    [SerializeField]
    TMP_Text _scoreText;

    public void SetScore(int score)
    {
        _scoreText.text = $"{score:000000}";
    }
}
