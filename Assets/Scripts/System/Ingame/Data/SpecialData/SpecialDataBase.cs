using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "EquipmentData/SpecialDataBase", fileName = "SpecialData")]
public class SpecialDataBase : ScriptableObject
{
    [SerializeField] private SpecialData[] _specials;
    [Header("初期開放必殺技はここにも追加")]
    [SerializeField] private SpecialData[] _initialOpenSpecial;
    private Dictionary<int, SpecialData> _specialDict;

    private void OnValidate()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        _specialDict = new Dictionary<int, SpecialData>();
        foreach (var special in _specials)
        {
            if (special != null)
            {
                _specialDict[special.ID] = special;
            }
        }
    }

    public void InitializeInitialOpenSpecials()
    {
        var data = new EquipmentData();
        foreach (var special in _initialOpenSpecial)
        {
            data.UnlockWeapon(special.ID);
        }
        SaveLoadService.Save(data);
    }

    public SpecialData GetSpecial(int weaponId)
    {
        if (_specialDict == null) InitializeDictionary();
        return _specialDict.TryGetValue(weaponId, out SpecialData weapon) ? weapon : null;
    }

    public SpecialData[] GetAllSpecials() => _specials;

#if UNITY_EDITOR
    [CustomEditor(typeof(SpecialDataBase))]
    public class SpecialDataLoad : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.Space(10);
            SpecialDataBase dataBase = (SpecialDataBase)target;

            if (GUILayout.Button("Initialize Unlock Specials"))
            {
                dataBase.InitializeInitialOpenSpecials();
            }
        }
    }
#endif
}
