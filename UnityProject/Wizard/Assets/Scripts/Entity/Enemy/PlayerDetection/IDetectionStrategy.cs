using Entity.Player;
using UnityEngine;
using Utils;

namespace Entity.Enemy.PlayerDetection
{
    public interface IDetectionStrategy
    {
        bool Execute(PlayerManager player, Transform detector, CountdownTimer timer);
    }
}