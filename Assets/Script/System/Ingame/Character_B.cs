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
        }
        public void HitHeal(float heal)
        {
            _data.Health += heal;
        }

        /// <summary>
        /// データを初期化する
        /// </summary>
        public void Initialize(DataType data)
        {
            //データを生成
            _data = Instantiate(data);
            _data.OnHealthChanged += HitDamage;
            _data.OnHealthChanged += HitHeal;
        }

        private void OnDestroy()
        {
            _data = null;
        }
    }
}
public interface IFightable
{
    void HitDamage(float damage);
}