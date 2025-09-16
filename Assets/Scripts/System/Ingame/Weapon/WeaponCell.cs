using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCell : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _valueText;
    [SerializeField] private TMP_Text _explanationText;
    [SerializeField] private Image _selectPanel;
    private WeaponData _weaponData;
    public WeaponData WeaponData => _weaponData;

    public void Initialize(Sprite icon,string name, string explanation, int value, WeaponData data)
    {
        _icon.sprite = icon;
        _nameText.text = name;
        _valueText.text = $"{value}";
        _explanationText.text = explanation;
        _weaponData = data;
    }

    public void Select()
    {
        var c = _selectPanel.color;
        c.a = 1;
        _selectPanel.color = c;
    }

    public void UnSelect()
    {
        var c = _selectPanel.color;
        c.a = 0;
        _selectPanel.color = c;
    }
}
