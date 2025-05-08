using UnityEngine;

[CreateAssetMenu(menuName = "ScoreData", fileName = "ScoreData")]
public class ScoreData : ScriptableObject
{
    [SerializeField]
    float _score;

    public float Score => _score;
}
