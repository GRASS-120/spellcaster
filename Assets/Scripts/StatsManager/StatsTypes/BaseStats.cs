using UnityEngine;

namespace StatsManager.StatsTypes
{
    // TODO: проблема этой системы в том, что ее очень проблемно расширять (например сделать разный набор статов, условно
    // отдельно для игрока и отедльно для врага... пока я на это забью, так как игра не про это, но как исправить?
    [CreateAssetMenu(fileName = "BaseStats", menuName = "Scriptable Objects/Stats/BaseStats", order = 0)]
    public class BaseStats : ScriptableObject
    {
        public float maxHp = 100;
        public float maxMana = 100;
        [Space][Space]
        public float attackDamage = 10;
        public float attackSpeed = 5;
        [Space][Space]
        public float moveSpeed = 8;
        [Space][Space]
        public float defence = 20;
    }
}