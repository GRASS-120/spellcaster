using Entity.Player;
using Player;

namespace Interactable
{
    public interface IItemVisitor
    {
        public void Visit(PlayerManager player);
    }
}