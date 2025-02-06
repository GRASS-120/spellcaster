namespace StatsManager.StatsTypes
{
    // TODO: мб сюда записывать вообще ВСЕ статы? не общие, а прям все. а то наследования нет у енамов
    public enum StatType 
    {
        MaxHp,
        MaxMana,
        AttackDamage,
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
        ///Максимальное количество очков здоровья
        /// </summary>
        public float MaxHp
        {
            get
            {
                var q = new Query(StatType.MaxHp, BaseStats.maxHp);
                Mediator.PerformQuery(this, q);
                return q.Value;
            }
        }
        
        /// <summary>
        ///Максимальное количество очков маны
        /// </summary>
        public float MaxMana
        {
            get
            {
                var q = new Query(StatType.MaxMana, BaseStats.maxMana);
                Mediator.PerformQuery(this, q);
                return q.Value;
            }
        }

        /// <summary>
        /// Сила атаки
        /// </summary>
        public float AttackDamage
        {
            get
            {
                var q = new Query(StatType.AttackDamage, BaseStats.attackDamage);
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
            return $"MaxHp: {MaxHp}, MaxMana: {MaxMana}, Attack: {AttackDamage}, AttackSpeed: {AttackSpeed}, " +
                   $"MoveSpeed: {MoveSpeed}, Defence: {Defence}";
        }
    }
}