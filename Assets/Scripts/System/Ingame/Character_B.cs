using UnityEngine;

namespace Script.System.Ingame
{
    public abstract class Character_B<DataType> : MonoBehaviour, IFightable
    where DataType : CharacterData_B
    {
        protected DataType _data;
        [SerializeField] Transform _targetCenter;
        [SerializeField] DamageReactionCollider _damageReactionCollider;
        public virtual void HitDamage(Collider other)
        {
            if (other.TryGetComponent(out Bullet_B component))
            {
                _data.Health -= component.AttackPower;
                if (_data.Health <= 0)
                {
                    Dead();
                }
            }
        }
        /// <summary>
        /// 増やすときは正の値、減らすときは負の値
        /// </summary>
        protected virtual bool GaugeValueChange(float value)
        {
            if (value < 0 && _data.Gauge + value < 0) return false;

            _data.Gauge = Mathf.Clamp(_data.Gauge + value, 0, _data.MaxGauge);
            return true;
        }

        protected virtual void OnHealthChanged(float health) { }
        protected virtual void OnGaugeChanged(float value) { }

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

        protected virtual void Start_B()
        {
            _damageReactionCollider.OnTriggerEnterEvent += HitDamage;
        }

        protected virtual void OnDestroyMethod()
        {
            _data = null;
            if (_data == null) return;
            _data.OnHealthChanged -= OnHealthChanged;
            _damageReactionCollider.OnTriggerEnterEvent -= HitDamage;
        }
        private void OnDestroy()
        {
            OnDestroyMethod();
        }


        protected virtual void Dead()
        {
            Debug.Log($"{gameObject.name}が死亡しました");
        }

        public Transform GetTargetCenter()
        {
            return _targetCenter;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public virtual bool IsTargetInView()
        {
            return true;
        }
    }
}