using SpaceOpera.Core.Military;

namespace SpaceOpera.View.Info
{
    public class WeaponDescriber : IDescriber
    {
        public void Describe(IEnumerable<object> objects, InfoPanel infoPanel)
        {
            var weapon = objects.First();

            foreach (var damageType in Enum.GetValues(typeof(DamageType)).Cast<DamageType>())
            {
                if (damageType == DamageType.Unknown)
                {
                    continue;
                }
                infoPanel.AddValues(
                    StringUtils.FormatEnumString(damageType.ToString()),
                    objects.Cast<Weapon>().Select(x => x.Damage.Amount[damageType]), "{0:0.##}");
            }
            infoPanel.AddValues("Penetration", objects.Cast<Weapon>().Select(x => x.Penetration), "{0:0.##}");
            infoPanel.AddValues("Accuracy", objects.Cast<Weapon>().Select(x => x.Accuracy), "{0:P0}");
            infoPanel.AddValues("Tracking", objects.Cast<Weapon>().Select(x => x.Tracking), "{0:P0}");
        }

        public void Describe(object @object, InfoPanel infoPanel)
        {
            Describe(Enumerable.Repeat(@object, 1), infoPanel);
        }
    }
}
