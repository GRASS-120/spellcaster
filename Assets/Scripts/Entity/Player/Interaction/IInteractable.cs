using Entity.Enemy;

namespace Entity.Player.Interaction
{
    public interface IInteractable
    {
        /// <summary>
        /// Взаимодействие при нажатии на E 
        /// </summary>
        /// <param name="player"></param>
        public void Interact(PlayerManager player);

        /// <summary>
        /// Взаимодействие при нажатии на E 
        /// </summary>
        /// <param name="player"></param>
        public void AltInteract(PlayerManager player);
    }
}