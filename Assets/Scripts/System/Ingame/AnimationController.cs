using Cysharp.Threading.Tasks;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    // ステート時間のリセットを行う閾値（秒）
    private const float NormalizedTimeResetThreshold = 10000f;

    private void Start()
    {
        ResetLongRunningState().Forget();
    }

    /// <summary>
    /// Animatorの内部で保存している値がすごく大きくなってしまう時があるので、それを回避するためのメソッド
    /// </summary>
    /// <returns></returns>
    private async UniTask ResetLongRunningState()
    {
        try
        {
            while (true)
            {
                var info = _animator.GetCurrentAnimatorStateInfo(0);

                // ループ回数としてnormalizedTimeを監視
                if (info.normalizedTime > NormalizedTimeResetThreshold)
                {
                    _animator.Play(info.shortNameHash, 0, 0f);
                }

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: destroyCancellationToken);
            }
        }
        catch { }
    }

    public void SetAttack(string name, AnimationType type)
    {
        switch (type)
        {
            case AnimationType.Bool:
                SetBool(name, true);
                break;
            case AnimationType.Trigger:
                SetTrigger(name);
                break;
        }
    }

    public void ResetAttack(string name, AnimationType type)
    {
        switch (type)
        {
            case AnimationType.Bool:
                SetBool(name, false);
                break;
            case AnimationType.Trigger:
                _animator.ResetTrigger(name);
                break;
        }
    }

    public void SetFloat(string name, float value)
    {
        _animator.SetFloat(name, value);
    }

    public void SetFloat(string name, float value, float dampTime)
    {
        _animator.SetFloat(name, value, dampTime, Time.deltaTime);
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

    public float GetFloat(string name) => _animator.GetFloat(name);
    public bool GetBool(string name) => _animator.GetBool(name);
    public int GetInt(string name) => _animator.GetInteger(name);

    public void PlayDead()
    {
        foreach (AnimatorControllerParameter param in _animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
                _animator.ResetTrigger(param.name);

            if (param.type == AnimatorControllerParameterType.Bool)
                _animator.SetBool(param.name, false);
        }

        // 例: 死亡アニメを再生
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
