using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] float _yOffset = 50;

    private void LateUpdate()
    {
        Vector3 newPos = _player.position;
        newPos.y = _player.position.y + _yOffset;
        transform.position = newPos;

        transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
