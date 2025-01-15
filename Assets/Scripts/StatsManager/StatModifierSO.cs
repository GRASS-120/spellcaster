using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using StatsManager.StatsTypes;
using UnityEngine;

namespace StatsManager
{
    [CreateAssetMenu(fileName = "StatModifier_", menuName = "Scriptable Objects/Stats/Stat Modifier", order = 1)]
    public class StatModifierSO : ScriptableObject
    {
        public enum OperationType { Add, Multiply, Subtraction, Division }
        [Serializable] public struct StatModifierParams {
            public StatType type;
            public OperationType operationType;
            public float value;
            public float duration;
        }

        [SerializeField] public List<StatModifierParams> statModifierList;
        
        public void ApplyModifierEffect(PlayerManager player, IHasStatModifier source)
        {
            foreach (var item in statModifierList) {
                var (modifier, priority) = item.operationType switch {
                    OperationType.Add => (new StatModifier(item.type, item.duration, v => v + item.value, source), 1),
                    OperationType.Multiply => (new StatModifier(item.type, item.duration, v => v * item.value, source), 2),
                    OperationType.Subtraction => (new StatModifier(item.type, item.duration, v => v - item.value, source), 3),
                    OperationType.Division => (new StatModifier(item.type, item.duration, v => (float)Math.Ceiling(v / item.value), source), 4),
                    _ => throw new ArgumentOutOfRangeException()
                };

                player.Stats.Mediator.AddModifier(modifier, priority);
            }
        }

        // навряд ли будет наблюдатся много статов, так что оптимизировать не стоит. но если нужно, то имеет
        // смысл сделать source в StatsMediator, так как в нем сразу можно обратится ко всем модам по источнику
        public void DisableModifierEffect(PlayerManager player, IHasStatModifier source)
        {
            var toRemove = new List<(StatModifier modifier, int priority)>();

            foreach (var kvp in player.Stats.Mediator.Modifiers)
            {
                var priority = kvp.Key;
                var list = kvp.Value;
        
                foreach (var modifier in list)
                {
                    if (modifier.Source == source)
                    {
                        toRemove.Add((modifier, priority));
                    }
                }
            }

            foreach (var (modifier, priority) in toRemove)
            {
                player.Stats.Mediator.RemoveModifier(modifier, priority);
            }
        }
    }
}