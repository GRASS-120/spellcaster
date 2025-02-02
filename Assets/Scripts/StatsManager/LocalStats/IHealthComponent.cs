namespace StatsManager.LocalStats
{
    public interface IHealthComponent
    {
        public float Hp { get; set; }
        
        public void TakeDamage(float damage);  // + damage dealer?
    }
}