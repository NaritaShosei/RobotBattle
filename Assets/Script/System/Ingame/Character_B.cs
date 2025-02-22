using UnityEngine;

namespace Script.System.Ingame
{
    public class Character_B<DataType> : MonoBehaviour
    where DataType : CharacterData_B
    {
        [SerializeField]
        protected DataType _data;
        
        /// <summary>
        /// データを初期化する
        /// </summary>
        public void Initialize()
        {
            //データを生成
            _data = Instantiate(_data);
        }

        private void OnDestroy()
        {
            _data = null;
        }
    }
}