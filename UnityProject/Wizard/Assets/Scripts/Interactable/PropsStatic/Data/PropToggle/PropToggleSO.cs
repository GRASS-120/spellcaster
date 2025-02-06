using UnityEngine;

namespace Interactable.PropsStatic.Data.PropToggle
{
    [CreateAssetMenu(fileName = "PST_NAME", menuName = "Scriptable Objects/Interactables/PropStatic/Toggle", order = 0)] 
    public class PropToggleSO : PropStaticSO
    {
        public Vector3 animatedObjectPos1;
        public Vector3 animatedObjectPos2;
        public float duration = 1f;
        public bool isLocked = false;
        // public PropLock lock;  - типо замок
        // public PropKey key;
    }
}