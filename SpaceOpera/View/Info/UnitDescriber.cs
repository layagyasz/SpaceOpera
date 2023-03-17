using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Military;

namespace SpaceOpera.View.Info
{
    public class UnitDescriber : IDescriber
    {
        public void DescribeAll(IEnumerable<object> objects, InfoPanel infoPanel)
        {
            Describe(objects.First(), infoPanel);
        }

        public void Describe(object @object, InfoPanel infoPanel)
        {
            Unit unit = (Unit)@object;

            infoPanel.AddValue("Hitpoints", unit.Hitpoints.ToString("0.##"));
            infoPanel.AddValue("Threat", unit.Threat.ToString("0.##"));
            infoPanel.AddValue("Detection", unit.Detection.ToString());
            infoPanel.AddValue("Evasion", unit.Evasion.ToString());
            infoPanel.AddValue("Maneuver", unit.Maneuver.ToString());
            if (unit.Command > 0)
            {
                infoPanel.AddValue("Threat", unit.Command.ToString("0.##"));
            }
            infoPanel.AddBreak();

            var capabilites = GetCapabilities(unit).ToList();
            if (capabilites.Count > 0)
            {
                infoPanel.AddHeader("Capabilities");
                foreach (var capability in capabilites)
                {
                    infoPanel.AddValue(capability.Key, capability.Value);
                }
            }

            infoPanel.AddHeader("Armor");
            new ArmorDescriber().Describe(unit.Armor, infoPanel);
            infoPanel.AddBreak();

            infoPanel.AddHeader("Shield");
            new ShieldDescriber().Describe(unit.Shield, infoPanel);
            infoPanel.AddBreak();

            foreach (var weapon in unit.Weapons.GetCounts())
            {
                infoPanel.AddHeader(weapon.Key.Name);
                infoPanel.AddValue("Count", weapon.Value.ToString());
                new WeaponDescriber().Describe(weapon.Key, infoPanel);
                infoPanel.AddBreak();
            }
        }

        private IEnumerable<KeyValuePair<string, string>> GetCapabilities(Unit Unit)
        {
            if (Unit.GetAttribute(ComponentAttribute.HangarSpace) > 0)
            {
                yield return new KeyValuePair<string, string>(
                    "Hangar Space", Unit.GetAttribute(ComponentAttribute.HangarSpace).ToString("0.##"));
            }
            if (Unit.GetAttribute(ComponentAttribute.CargoSpace) > 0)
            {
                yield return new KeyValuePair<string, string>(
                    "Cargo Space", Unit.GetAttribute(ComponentAttribute.CargoSpace).ToString("0.##"));
            }
        }
    }
}