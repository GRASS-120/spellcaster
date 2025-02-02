using Entity.Enemy;
using Entity.Player;
using UnityEngine;

namespace Interactable.Items.Tool
{
    // HandleAction вызывает только игрок при нажатии кнопки... как сделать эту систему универсальной??? чтобы
    // и игрок и враг могли (по крайней мере с оружием)
    public class ToolWeapon : Tool
    {
        public override void HandleAction(PlayerManager player)
        {
            Debug.Log("kinut weapon");
            itemManager.StopClipping();
            itemManager.ThrowObject();
        }
        // protected override void HandleAction(EnemyManager enemy)
        // {
        //     // Debug.Log
        // }
    }
}