namespace Entity.Player.Interaction
{
    public interface IInteractable
    {
        public void Interact(PlayerManager player);
        public void AltInteract(PlayerManager player);
    }
}