using Cardamom.Trackers;

namespace SpaceOpera.Core.Economics
{
    public class Inventory
    {
        public enum ChangeStatus
        {
            Unknown,
            InProgress,
            Blocked,
            Done
        }

        public float Size => _space.MaxAmount;
        public float Used => _space.Amount;
        public float Remaining => _space.Remaining;
        public MultiQuantity<IMaterial> Contents { get; } = new();

        private readonly Pool _space;

        public Inventory(float size)
        {
            _space = new Pool(size, /* startFull= */ false);
        }

        public bool Contains(MultiQuantity<IMaterial> materials)
        {
            return Contents.All(x => x.Value >= materials.Get(x.Key));
        }

        public bool IsFull()
        {
            return _space.IsFull();
        }

        public bool IsEmpty()
        {
            return _space.IsEmpty();
        }

        public void SetSize(float size)
        {
            if (size < Used)
            {
                // Dump contents.
            }
            _space.SetMax(size);
        }

        public bool TryAdd(MultiQuantity<IMaterial> materials)
        {
            bool done = true;
            foreach (var m in materials)
            {
                done &= TryAdd(m.Key, m.Value);
            }
            return done;
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
                Contents.Add(cost.Key, -maxUnits * cost.Value);
            }
            return maxUnits;
        }

        public ChangeStatus MaxTransferFrom(Inventory other, MultiQuantity<IMaterial> materials, float maxTransfer)
        {
            bool done = true;
            bool worked = false;
            foreach (var material in materials)
            {
                var remaining = material.Value - Contents.Get(material.Key);
                if (remaining > 0)
                {
                    if (maxTransfer > 0)
                    {
                        var maxSpace = Math.Min(_space.Remaining, maxTransfer);
                        var maxUnits =
                            Math.Min(
                                Math.Min(remaining, other.Contents.Get(material.Key)), maxSpace / material.Key.Size);
                        other.TryRemove(material.Key, maxUnits);
                        TryAdd(material.Key, maxUnits);
                        var used = maxUnits * material.Key.Size;
                        maxTransfer -= used;
                        if (used > 0)
                        {
                            worked = true;
                        }
                        if (remaining - maxUnits > 0)
                        {
                            done = false;
                        }
                    }
                    else
                    {
                        done = false;
                        break;
                    }
                }
            }
            if (done)
            {
                return ChangeStatus.Done;
            }
            return worked ? ChangeStatus.InProgress : ChangeStatus.Blocked;
        }

        public ChangeStatus MaxTransferTo(Inventory other, float maxTransfer)
        {
            bool done = true;
            bool worked = false;
            foreach (var material in Contents)
            {
                if (material.Value > 0)
                {
                    if (maxTransfer > 0)
                    {
                        var maxSpace = Math.Min(other.Remaining, maxTransfer);
                        var maxUnits = Math.Min(material.Value, maxSpace / material.Key.Size);
                        other.TryAdd(material.Key, maxUnits);
                        TryRemove(material.Key, maxUnits);
                        var used = maxUnits * material.Key.Size;
                        maxTransfer -= used;
                        if (used > 0)
                        {
                            worked = true;
                        }
                        if (material.Value - maxUnits > 0)
                        {
                            done = false;
                        }
                    }
                    else
                    {
                        done = false;
                        break;
                    }
                }
            }
            if (done)
            {
                return ChangeStatus.Done;
            }
            return worked ? ChangeStatus.InProgress : ChangeStatus.Blocked;
        }
    }
}
