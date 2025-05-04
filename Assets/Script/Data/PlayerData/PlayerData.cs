using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "GameData/CharacterData/PlayerData")]
public class PlayerData : CharacterData_SB
{
    [SerializeField]
    float _guardMinValue = 200;
    public float GuardMinValue => _guardMinValue;
}
