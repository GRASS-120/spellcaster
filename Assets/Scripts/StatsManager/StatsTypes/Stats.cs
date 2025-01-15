namespace StatsManager.StatsTypes
{
    public enum StatType 
    {
        HP,
        Mana,
        Attack,
        AttackSpeed,
        MoveSpeed,
        Defence
    }
    
    public class Stats
    {
        public readonly StatsMediator Mediator;
        public readonly BaseStats BaseStats;

        public Stats(StatsMediator mediator, BaseStats baseStats)
        {
            Mediator = mediator;
            BaseStats = baseStats;
        }

        /// <summary>
        /// Очки здоровья
        /// </summary>
        public float HP
        {
            get
            {
                var q = new Query(StatType.HP, BaseStats.hp);
                Mediator.PerformQuery(this, q);
                return q.Value;
            }
        }

        /// <summary>
        /// Очки маны
        /// </summary>
        public float Mana
        {
            get
            {
                var q = new Query(StatType.Mana, BaseStats.mana);
                Mediator.PerformQuery(this, q);
                return q.Value;
            }
        }

        /// <summary>
        /// Сила атаки
        /// </summary>
        public float Attack
        {
            get
            {
                var q = new Query(StatType.Attack, BaseStats.attack);
                Mediator.PerformQuery(this, q);
                return q.Value;
            }
        }

        /// <summary>
        /// Скорость атаки
        /// </summary>
        public float AttackSpeed
        {
            get
            {
                var q = new Query(StatType.AttackSpeed, BaseStats.attackSpeed);
                Mediator.PerformQuery(this, q);
                return q.Value;
            }
        }

        /// <summary>
        /// Обычная скорость перемещения
        /// </summary>
        public float MoveSpeed
        {
            get
            {
                var q = new Query(StatType.MoveSpeed, BaseStats.moveSpeed);
                Mediator.PerformQuery(this, q);
                return q.Value;
            }
        }

        /// <summary>
        /// Защита (броня)
        /// </summary>
        public float Defence
        {
            get
            {
                var q = new Query(StatType.Defence, BaseStats.defence);
                Mediator.PerformQuery(this, q);
                return q.Value;
            }
        }

        public override string ToString() 
        {
            return $"HP: {HP}, Mana: {Mana}, Attack: {Attack}, AttackSpeed: {AttackSpeed}, " +
                   $"MoveSpeed: {MoveSpeed}, Defence: {Defence}";
        }
    }
}