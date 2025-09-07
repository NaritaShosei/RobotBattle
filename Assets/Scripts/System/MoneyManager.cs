using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public MoneyData MoneyData { get; private set; }

    private void Awake()
    {
        MoneyData = new MoneyData();
        ServiceLocator.Set(this);
    }
}

[System.Serializable]
public class MoneyData
{
    public int Money { get; private set; }
    public void AddMoney(int money)
    {
        Money += money;
    }
    public void UseMoney(int money)
    {
        Money -= money;
    }
    public bool IsUseMoney(int money)
    {
        return (Money - money) >= 0;
    }
}