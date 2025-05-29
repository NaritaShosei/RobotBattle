using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField]
    Animator _animator;

    public void SetFloat(string name, float value)
    {
        _animator.SetFloat(name, value);
    }

    public void SetBool(string name, bool value)
    {
        _animator.SetBool(name, value);
    }

    public void SetInteger(string name, int value)
    {
        _animator.SetInteger(name, value);
    }

    public void SetTrigger(string name)
    {
        _animator.SetTrigger(name);
    }

    public void PlayDead()
    {
        foreach (AnimatorControllerParameter param in _animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                _animator.ResetTrigger(param.name);
            }

            if (param.type == AnimatorControllerParameterType.Bool)
            {
                _animator.SetBool(_animator.name, false);
            }
        }
        // _animator.SetBool("死", true);
    }

    public void SetWeight(AnimationLayer layer, float weight)
    {
        _animator.SetLayerWeight((int)layer, weight);
    }

}
public enum AnimationLayer
{
    Base = 0,
    Attack = 1,
}