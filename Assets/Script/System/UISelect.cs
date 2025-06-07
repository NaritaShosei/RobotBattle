using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UISelect : MonoBehaviour
{
    [SerializeField] Image[] _selects;
    int _selectIndex;
    InputManager _input;
    private void Awake()
    {
        _input = ServiceLocator.Get<InputManager>();
        _input.UISelectAction.performed += Select;
    }

    void Select(InputAction.CallbackContext context)
    {
        Debug.LogError("SELECT");
        _input.SwitchInputMode(ModeType.Player);
        ServiceLocator.Get<InGameManager>().PauseResume();
    }
    private void OnDisable()
    {
        _input.UISelectAction.performed -= Select;
    }
}
