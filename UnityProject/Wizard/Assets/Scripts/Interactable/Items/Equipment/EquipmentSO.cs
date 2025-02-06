using StatsManager;
using UnityEngine;

namespace Player.Items
{
    [CreateAssetMenu(fileName = "E_NAME", menuName = "Scriptable Objects/Interactables/Items/Equipment", order = 0)]

    public class EquipmentSO : ItemSO
    {
        public StatModifierSO modifiers;
    }
}