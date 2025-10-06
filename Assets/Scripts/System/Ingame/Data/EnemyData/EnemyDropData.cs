using UnityEngine;

[CreateAssetMenu(menuName = "GameData/EnemyDropData", fileName = "EnemyDropData")]
public class EnemyDropData : ScriptableObject
{
    [SerializeField]
    private int _score = 100;

    [SerializeField]
    private int _money  = 100;
    public int Score => _score;
    public int Money => _money;
}
