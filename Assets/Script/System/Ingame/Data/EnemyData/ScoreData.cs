using UnityEngine;

[CreateAssetMenu(menuName = "ScoreData", fileName = "ScoreData")]
public class ScoreData : ScriptableObject
{
    [SerializeField]
    int _score;

    public int Score => _score;
}
