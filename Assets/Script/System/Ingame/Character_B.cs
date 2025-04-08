using UnityEngine;

namespace Script.System.Ingame
{
    public abstract class Character_B<DataType> : MonoBehaviour, IFightable
    where DataType : CharacterData_B
    {
        [SerializeField]
        protected DataType _data;

        protected float MaxHealth { get => _data.MaxHealth; }
        protected float _currentHealth;
        public void HitDamage()
        {

        }

        /// <summary>
        /// データを初期化する
        /// </summary>
        public void Initialize(DataType data)
        {
            //データを生成
            _data = Instantiate(data);
        }

        private void OnDestroy()
        {
            _data = null;
        }
    }
}
public interface IFightable
{
    void HitDamage();
}