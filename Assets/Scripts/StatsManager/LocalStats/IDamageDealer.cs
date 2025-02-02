using UnityEngine;

namespace StatsManager.LocalStats
{
    public interface IDamageDealer
    {
        public Transform DamageSource { get; set; }
    }
}