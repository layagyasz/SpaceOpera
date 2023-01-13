namespace SpaceOpera.Core.Economics
{
    public class ResourceNode
    {
        public IMaterial Resource { get; }
        public int Amount { get; }

        public ResourceNode(IMaterial resource, int amount)
        {
            Resource = resource;
            Amount = amount;
        }
    }
}
