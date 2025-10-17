using TMPro;
using UnityEngine;

public class WeaponExplanation : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _power;
    [SerializeField] private TMP_Text _rate;

    public void Set(WeaponData data)
    {
        _name.text = data.WeaponName;
        _power.text = $"{data.AttackPower}";
        _rate.text = $"{data.AttackRate}";
    }
}
