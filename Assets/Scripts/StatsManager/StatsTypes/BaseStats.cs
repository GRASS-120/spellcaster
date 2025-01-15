using UnityEngine;

namespace StatsManager.StatsTypes
{
    // TODO: сделать asset menu в дочерних только -> пока пох на разделение статов.
    
    [CreateAssetMenu(fileName = "BaseStats", menuName = "Scriptable Objects/Stats/BaseStats", order = 0)]
    public class BaseStats : ScriptableObject
    {
        // local
        public float hp = 100;
        public float mana = 100;
        [Space][Space]
        public float attack = 10;
        public float attackSpeed = 5;
        [Space][Space]
        public float moveSpeed = 8;
        [Space][Space]
        public float defence = 20;
    }
}