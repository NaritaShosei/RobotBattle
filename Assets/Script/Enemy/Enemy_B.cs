using Script.System.Ingame;
using UnityEngine;

public class Enemy_B : Character_B<CharacterData_B>
{
    [SerializeField] CharacterData_B _dataBase;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize(_dataBase);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(_data.Health);
    }
}
