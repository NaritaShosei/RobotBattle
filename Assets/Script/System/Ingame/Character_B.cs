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
        /// <summary>
        /// 増やすときは正の値、減らすときは負の値
        /// </summary>
        protected virtual bool GaugeValueChange(float value)
        {
            if (value < 0 && _data.Gauge + value <= 0) return false;

            _data.Gauge = Mathf.Clamp(_data.Gauge + value, 0, _data.MaxGauge);
            return true;
        }

        protected virtual void OnHealthChanged(float health)
        {

        }
        protected virtual void OnGaugeChanged(float value)
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
            _data.Gauge = data.MaxGauge;
            _data.OnGaugeChanged += OnGaugeChanged;
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