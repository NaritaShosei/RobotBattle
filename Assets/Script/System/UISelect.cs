using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UISelect : MonoBehaviour
{
    [SerializeField] Image[] _selects;
    int _selectIndex;
    InputManager _input;
    [SerializeField] SortType _sortType;

    enum SortType
    {
        vertical,
        horizontal,
    }

    private void OnEnable()
    {
        _input = ServiceLocator.Get<InputManager>();
        _input.UISelectAction.started += Navigate;
        if (_selects.Length <= 0) { return; }

        switch (_sortType)
        {
            //Y軸を基準に降順に並び替え
            case SortType.vertical:
                _selects = _selects.OrderByDescending(x => x.transform.position.y).ToArray();
                break;
            //X軸を基準に昇順に並び替え
            case SortType.horizontal:
                _selects = _selects.OrderBy(x => x.transform.position.x).ToArray();
                break;
        }
        //選択しているImageの色を変更
        _selects[_selectIndex].color = Color.red;
    }
    private void OnDisable()
    {
        _input.UISelectAction.started -= Navigate;
    }

    void Navigate(InputAction.CallbackContext context)
    {
        //現在のインデックスのImageの色を白に
        _selects[_selectIndex].color = Color.white;

        //入力を保持
        Vector2 input = context.ReadValue<Vector2>();
        //ソートの縦と横で参照する値を変える
        switch (_sortType)
        {
            //縦の時
            case SortType.vertical:
                if (input.y >= 0.5f)
                {
                    MoveIndex(-1);
                }
                else if (input.y <= -0.5f)
                {
                    MoveIndex(1);
                }
                break;

            //横の時
            case SortType.horizontal:
                if (input.x >= 0.5f)
                {
                    MoveIndex(1);
                }
                else if (input.x <= -0.5f)
                {
                    MoveIndex(-1);
                }
                break;
        }
        Debug.Log($"現在の添字{_selectIndex}");
        //移動したインデックスに対応するImageの色を赤に
        _selects[_selectIndex].color = Color.red;
    }

    void Submit(InputAction.CallbackContext context)
    {

    }
    /// <summary>
    /// インデックスを移動させる。
    /// </summary>
    /// <param name="dir">移動する方向</param>
    void MoveIndex(int dir)
    {
        _selectIndex += dir;
        _selectIndex = (_selectIndex + _selects.Length) % _selects.Length;
    }
}
