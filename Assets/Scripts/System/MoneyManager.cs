using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    private MoneyData _moneyData;

    private void Awake()
    {
        _moneyData = SaveLoadService.Load<MoneyData>();
        ServiceLocator.Set(this);
    }

    public void AddMoney(int amount)
    {
        _moneyData .AddMoney(amount);
        SaveLoadService.Save(_moneyData);
    }

    public void UseMoney(int amount)
    {
        _moneyData.UseMoney(amount);
        SaveLoadService.Save(_moneyData);
    }

    public bool CanUseMoney(int amount)
    {
        return _moneyData.CanUseMoney(amount);
    }

    public int GetMoney()
    {
        return _moneyData.Money;
    }
}

[System.Serializable]
public class MoneyData
{
    [SerializeField] private int _money; 
    public int Money => _money;
    public void AddMoney(int money)
    {
        _money += money;
    }
    public void UseMoney(int money)
    {
        _money -= money;
    }
    public bool CanUseMoney(int money)
    {
        return (_money - money) >= 0;
    }
}