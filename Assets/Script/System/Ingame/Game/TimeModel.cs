using UnityEngine;

public class TimeModel
{
    float _currentTime;
    readonly int _maxTime;

    public float CurrentTime => _currentTime;
    public bool IsTimeOver => _currentTime <= 0;

    public TimeModel(int maxTime)
    {
        _maxTime = maxTime;
        _currentTime = maxTime;
    }

    public void UpdateTime(float deltaTime)
    {
        _currentTime = Mathf.Max(_currentTime - deltaTime, 0);
    }

    public void Reset()
    {
        _currentTime = _maxTime;
    }
}
