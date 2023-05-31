namespace SpaceOpera.Core.Orders
{
    public interface IOrder
    {
        ValidationFailureReason Validate(World world);
        bool Execute(World world);
    }
}
