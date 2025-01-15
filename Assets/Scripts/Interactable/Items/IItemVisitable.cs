namespace Interactable
{
    public interface IItemVisitable
    {
        public void Accept(IItemVisitor visitor);
    }
}