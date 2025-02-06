using System;
using System.Collections.Generic;
using System.Linq;
using StatsManager.StatsTypes;
using UnityEngine;

namespace StatsManager
{
    public class Query
    {
        public readonly StatType StatType;
        public float Value;

        public Query(StatType statType, float value)
        {
            this.StatType = statType;
            this.Value = value;
        }
    }
    
    public class StatsMediator
    {
        public event EventHandler OnStatChange;
        public event EventHandler OnDisposeModifier;
        public event EventHandler OnAddModifier;
        
        public IReadOnlyDictionary<int, List<StatModifier>> Modifiers => modifiers;

        private readonly SortedDictionary<int, List<StatModifier>> modifiers = new();
        
        public void PerformQuery(object sender, Query query)
        {
            foreach (var kvp in modifiers.OrderBy(kvp => kvp.Key))
            {
                foreach (var modifier in kvp.Value)
                {
                    modifier.Handle(sender, query);
                }
            }
        }
        public void AddModifier(StatModifier modifier, int priority) 
        {
            if (!modifiers.ContainsKey(priority))
            {
                modifiers[priority] = new List<StatModifier>();
            }
            
            modifiers[priority].Add(modifier);

            OnStatChange?.Invoke(this, EventArgs.Empty);
            OnAddModifier?.Invoke(this, EventArgs.Empty);

            modifier.OnDispose += _ =>
            {
                RemoveModifier(modifier, priority);
            };
        }

        public void RemoveModifier(StatModifier modifier, int priority)
        {
            if (modifiers.ContainsKey(priority))
            {
                modifiers[priority].Remove(modifier);
                if (modifiers[priority].Count == 0)
                {
                    modifiers.Remove(priority);
                }
            }
        }

        // Должен вызываться обладателем статов в Update() для обновления состояния всех статов
        public void Update(float deltaTime)
        {
            // Создаем список модификаторов для удаления
            var modifiersToRemove = new List<(int priority, StatModifier modifier)>();

            // Обновляем таймеры у всех модификаций с ограничением по времени
            foreach (var kvp in modifiers)
            {
                var priority = kvp.Key;
                var modifiersList = kvp.Value;
                foreach (var modifier in modifiersList)
                {
                    modifier.Update(deltaTime);
                    
                    // если таймер иссяк (markedForRemoval = true), то добавляем их в список на удаление
                    if (modifier.MarkedForRemoval)
                    {
                        modifiersToRemove.Add((priority, modifier));
                        modifier.Dispose();
                        OnStatChange?.Invoke(this, EventArgs.Empty);
                        OnDisposeModifier?.Invoke(this, EventArgs.Empty);
                    }
                }
            }

            // Удаляем модификаторы
            foreach (var (priority, modifier) in modifiersToRemove)
            {
                RemoveModifier(modifier, priority);
            }
        }
    }
}
