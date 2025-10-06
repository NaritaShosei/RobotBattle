using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "GameData/EquipmentData/Database", fileName = "Database")]
public class EquipmentDatabase : ScriptableObject
{
    [Serializable]
    private class Data<T> where T : class, IData
    {
        [Header("全データ")]
        [SerializeField] private T[] _allData;

        [Header("初期開放データ")]
        [SerializeField] private T[] _initialData;

        private Dictionary<int, T> _dict;

        public void BuildDictionary()
        {
            _dict = new Dictionary<int, T>();
            foreach (var data in _allData)
            {
                if (data != null)
                {
                    _dict[data.ID] = data;
                }
            }
        }

        public T Get(int id)
        {
            if (_dict == null) BuildDictionary();
            return _dict.TryGetValue(id, out T data) ? data : null;
        }

        public T[] GetAll() => _allData;

        public void InitializeOpen(EquipmentData data, Action<EquipmentData, int> onUnlock)
        {
            foreach (var item in _initialData)
            {
                onUnlock?.Invoke(data, item.ID);
            }
        }
    }

    [SerializeField] Data<WeaponData> _weapons = new();
    [SerializeField] Data<SpecialData> _specials = new();

    private void OnValidate()
    {
        _weapons.BuildDictionary();
        _specials.BuildDictionary();
    }

    public WeaponData GetWeapon(int id) => _weapons.Get(id);
    public WeaponData[] GetAllWeaponData() => _weapons.GetAll();

    public SpecialData GetSpecial(int id) => _specials.Get(id);
    public SpecialData[] GetAllSpecialData() => _specials.GetAll();

    public void InitializeOpen()
    {
        var data = new EquipmentData();

        _weapons.InitializeOpen(data, (data, id) => data.UnlockWeapon(id));
        _specials.InitializeOpen(data, (data, id) => data.UnlockSpecial(id));

        SaveLoadService.Save(data);
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(EquipmentDatabase))]
    public class WeaponDataLoad : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.Space(10);
            EquipmentDatabase dataBase = (EquipmentDatabase)target;

            if (GUILayout.Button("装備データ初期化"))
            {
                dataBase.InitializeOpen();
            }
        }
    }
#endif
}

public interface IData
{
    int ID { get; }
}