using Cardamom.Trackers;

namespace SpaceOpera.Core.Economics
{
    public class Inventory
    {
        public float Size => _space.MaxAmount;
        public float Used => _space.Amount;
        public float Remaining => _space.Remaining;
        public MultiQuantity<IMaterial> Contents { get; } = new();

        private readonly Pool _space;

        public Inventory(float size)
        {
            _space = new Pool(size, /* startFull= */ false);
        }

        public bool IsFull()
        {
            return _space.IsFull();
        }

        public void SetSize(float size)
        {
            if (size < Used)
            {
                // Dump contents.
            }
            _space.SetMax(size);
        }

        public bool TryAdd(IMaterial material, float amount)
        {
            float addedUse = amount * material.Size;
            if (addedUse + Used > Size)
            {
                return false;
            }
            Contents.Add(material, amount);
            _space.Change(addedUse);
            return true;
        }

        public bool TryRemove(IMaterial material, float amount)
        {
            if (Contents.TryGetValue(material, out var currentAmount))
            {
                if (amount > currentAmount)
                {
                    return false;
                }
            }
            Contents.Add(material, -amount);
            _space.Change(-amount * material.Size);
            return true;
        }

        public float MaxAdd(IMaterial material, float amount)
        {
            float maxUnits = Remaining / material.Size;
            float units = Math.Min(amount, maxUnits);
            Contents.Add(material, units);
            _space.Change(units * material.Size);
            return amount - units;
        }

        public float MaxSpend(MultiQuantity<IMaterial> unitCost, float maxUnits)
        {
            foreach (var cost in unitCost)
            {
                maxUnits = Math.Min(Contents.Get(cost.Key) / cost.Value, maxUnits);
            }
            foreach (var cost in unitCost)
            {
                Contents.Add(cost.Key, maxUnits * cost.Value);
            }
            return maxUnits;
        }

        public bool MaxTransferFrom(Inventory other, MultiQuantity<IMaterial> materials, float maxTransfer)
        {
            float used = 0;
            foreach (var material in materials)
            {
                var remaining = material.Value - Contents.Get(material.Key);
                if (remaining > 0)
                {
                    var maxSpace = Math.Min(_space.Remaining, maxTransfer - used);
                    var maxUnits = Math.Min(other.Contents.Get(material.Key), maxSpace / material.Key.Size);
                    other.TryRemove(material.Key, maxUnits);
                    TryAdd(material.Key, maxUnits);
                    used += maxSpace;
                }
                if (Math.Abs(used - maxTransfer) < float.Epsilon)
                {
                    return true;
                }
            }
            return used > 0;
        }

        public bool MaxTransferTo(Inventory other, float maxTransfer)
        {
            float used = 0;
            foreach (var material in Contents)
            {
                if (material.Value > 0)
                {
                    var maxSpace = Math.Min(other.Remaining, maxTransfer - used);
                    var maxUnits = Math.Min(material.Value, maxSpace / material.Key.Size);
                    other.TryAdd(material.Key, maxUnits);
                    TryRemove(material.Key, maxUnits);
                    used += maxSpace;
                }
                if (Math.Abs(used - maxTransfer) < float.Epsilon)
                {
                    return true;
                }
            }
            return used > 0;
        }
    }
}
