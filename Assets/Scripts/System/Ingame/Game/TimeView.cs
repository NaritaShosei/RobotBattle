using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeView : MonoBehaviour
{
    [SerializeField]
    TMP_Text _text;

    public void SetTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        _text.text = $"{minutes:00}:{seconds:00}";
    }
}
