using UnityEngine;
using Utils;

namespace Entity.Enemy
{
    public interface IDetectionStrategy
    {
        bool Execute(Transform player, Transform detector, CountdownTimer timer);
    }
}