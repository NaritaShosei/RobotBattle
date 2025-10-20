using TMPro;
using UnityEngine;

public class WeaponExplanation : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _power;
    [SerializeField] private TMP_Text _rate;

    public void Set(WeaponData data)
    {
        // データがnullだったら空文字を表示
        _name.text = data ? data.WeaponName : "";
        _power.text = data ? $"{data.AttackPower}" : "";
        _rate.text = data ? $"{data.AttackRate}" : "";
    }
}
