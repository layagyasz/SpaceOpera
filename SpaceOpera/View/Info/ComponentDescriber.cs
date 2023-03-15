using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Military;

namespace SpaceOpera.View.Info
{
    public class ComponentDescriber : IDescriber
    {
        private static readonly HashSet<ComponentAttribute> s_BaseAttributes =
            new()
            {
                ComponentAttribute.ProductionCost,
                ComponentAttribute.Size
            };

        public bool IncludeBaseInfo { get; }

        public ComponentDescriber(bool IncludeBaseInfo)
        {
            this.IncludeBaseInfo = IncludeBaseInfo;
        }

        public void Describe(IEnumerable<object> objects, InfoPanel infoPanel)
        {
            var component = (IComponent)objects.First()!;

            switch (component.Slot.Type)
            {
                case ComponentType.BattalionTemplate:
                case ComponentType.DivisionTemplate:
                    new FormationTemplateDescriber().Describe(objects.Cast<IFormationTemplate>().ToList(), infoPanel);
                    return;
                case ComponentType.Infantry:
                case ComponentType.Ship:
                    new UnitDescriber().Describe(objects.Cast<Unit>().ToList(), infoPanel);
                    return;
                case ComponentType.PersonalWeapon:
                case ComponentType.ShipWeapon:
                case ComponentType.ShipMissile:
                    new WeaponDescriber()
                        .Describe(objects.Cast<IComponent>().Select(Weapon.FromComponent).ToList(), infoPanel);
                    return;
                case ComponentType.ShipShield:
                    new ShieldDescriber()
                        .Describe(objects.Cast<IComponent>().Select(Shield.FromComponent).ToList(), infoPanel);
                    return;
            }

            foreach (var attribute in component.Attributes.Select(x => x.Key).OrderBy(x => x.ToString()))
            {
                if (IncludeBaseInfo || !s_BaseAttributes.Contains(attribute))
                {
                    infoPanel.AddValues(
                        StringUtils.FormatEnumString(attribute.ToString()), 
                        objects.Cast<IComponent>().Select(x => x.GetAttribute(attribute)), "{0:N0}");
                }
            }
        }

        public void Describe(object @object, InfoPanel infoPanel)
        {
            Describe(Enumerable.Repeat(@object, 1), infoPanel);
        }
    }
}