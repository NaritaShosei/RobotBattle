using UnityEngine;
using UnityEngine.UI;

public class CrosshairView : MonoBehaviour
{
    [SerializeField] RectTransform _normalCrosshair;
    [SerializeField] RectTransform _lockOnCrosshair;
    //[SerializeField] Image _crosshairImage;
    //[SerializeField] Image _lockOnImage;
    public Vector2 CrosshairPos => _normalCrosshair.position;

    public void SetLockOn(bool isLocKOn)
    {
        _normalCrosshair.gameObject.SetActive(!isLocKOn);
        _lockOnCrosshair.gameObject.SetActive(isLocKOn);
    }

    public void SetLockOnPosition(Vector3 worldPosition)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        _lockOnCrosshair.position = screenPos;
    }
}
