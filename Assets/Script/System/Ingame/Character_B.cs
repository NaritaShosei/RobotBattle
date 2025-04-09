using UnityEngine;

namespace Script.System.Ingame
{
    public abstract class Character_B<DataType> : MonoBehaviour, IFightable
    where DataType : CharacterData_B
    {
        protected DataType _data;

        public void HitDamage(float damage)
        {
            _data.Health -= damage;
            if (_data.Health <= 0)
            {
                Debug.Log("Dead");
            }
        }
        public void HitHeal(float heal)
        {
            _data.Health += heal;
        }
        protected virtual void OnHealthChanged(float health)
        {

        }

        /// <summary>
        /// データを初期化する
        /// </summary>
        protected virtual void Initialize(DataType data)
        {
            //データを生成
            _data = Instantiate(data);
            _data.Health = data.MaxHealth;
            _data.OnHealthChanged += OnHealthChanged;
        }
        protected virtual void OnDestroyMethod()
        {
            _data = null;
            if (_data == null) return;
            _data.OnHealthChanged -= OnHealthChanged;
        }
        private void OnDestroy()
        {
            OnDestroyMethod();
        }
    }
    public interface IFightable
    {
        void HitDamage(float damage);
        void HitHeal(float heal);
    }
}