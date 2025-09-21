using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "EquipmentData/WeaponDatabase", fileName = "WeaponDatabase")]
public class WeaponDatabase : ScriptableObject
{
    [SerializeField] private WeaponData[] _weapons;
    [Header("初期開放武器はここにも追加")]
    [SerializeField] private WeaponData[] _initialOpenWeapon;
    private Dictionary<int, WeaponData> _weaponDict;

    private void OnValidate()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        _weaponDict = new Dictionary<int, WeaponData>();
        foreach (var weapon in _weapons)
        {
            if (weapon != null)
            {
                _weaponDict[weapon.WeaponID] = weapon;
            }
        }
    }

    public void InitializeInitialOpenWeapons()
    {
        var data = new EquipmentData();
        foreach (var weapon in _initialOpenWeapon)
        {
            data.UnlockWeapon(weapon.WeaponID);
        }
        SaveLoadService.Save(data);
    }

    public WeaponData GetWeapon(int weaponId)
    {
        if (_weaponDict == null) InitializeDictionary();
        return _weaponDict.TryGetValue(weaponId, out var weapon) ? weapon : null;
    }

    public WeaponData[] GetAllWeapons() => _weapons;

#if UNITY_EDITOR
    [CustomEditor(typeof(WeaponDatabase))]
    public class WeaponDataLoad : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.Space(10);
            WeaponDatabase dataBase = (WeaponDatabase)target;

            if (GUILayout.Button("Initialize Unlock Weapons"))
            {
                dataBase.InitializeInitialOpenWeapons();
            }
        }
    }
#endif
}
