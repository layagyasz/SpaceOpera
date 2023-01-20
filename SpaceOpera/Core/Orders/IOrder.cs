namespace SpaceOpera.Core.Orders
{
    public interface IOrder
    {
        ValidationFailureReason Validate();
        bool Execute(World world);
    }
}
