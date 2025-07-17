using UnityEngine;

public class GameManager : MonoBehaviour
{
    int _money;
    private void Awake()
    {
        ServiceLocator.Set(this);
    }

    void Start()
    {

    }

    public void AddMoney(int money)
    {
        _money += money;
    }

    public void RemoveMoney(int money)
    {
        _money -= money;
    }
    /// <summary>
    /// お金を使った際に0を下回るか判定
    /// </summary>
    /// <param name="money"></param>
    /// <returns>0以上だと true</returns>
    public bool IsUseMoney(int money)
    {
        return _money - money >= 0;
    }

    public int GetMoney()
    {
        return _money;
    }
}
