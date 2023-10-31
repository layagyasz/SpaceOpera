using Cardamom.Trackers;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
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

        public ComponentDescriber(bool includeBaseInfo)
        {
            IncludeBaseInfo = includeBaseInfo;
        }

        public void DescribeAll(IEnumerable<object> objects, InfoPanel infoPanel)
        {
            var component = (IComponent)objects.First()!;

            switch (component.Slot.Type)
            {
                case ComponentType.BattalionTemplate:
                case ComponentType.DivisionTemplate:
                    new FormationTemplateDescriber().DescribeAll(
                        objects.Cast<IFormationTemplate>().ToList(), infoPanel);
                    break;
                case ComponentType.Infantry:
                case ComponentType.Ship:
                    new UnitDescriber().DescribeAll(objects.Cast<Unit>().ToList(), infoPanel);
                    break;
                case ComponentType.SmallArm:
                case ComponentType.HeavyGun:
                case ComponentType.HeavyMissile:
                    new WeaponDescriber()
                        .DescribeAll(objects.Cast<IComponent>().Select(Weapon.FromComponent).ToList(), infoPanel);
                    break;
                case ComponentType.ShipShield:
                    new ShieldDescriber()
                        .DescribeAll(objects.Cast<IComponent>().Select(Shield.FromComponent).ToList(), infoPanel);
                    break;
                default:
                    DescribeDefault(objects.Cast<IComponent>(), infoPanel);
                    break;
            }

            if (component is DesignedMaterial material)
            {
                infoPanel.AddValue("Production Cost", material.ProductionCost.ToString("N0"));
                infoPanel.AddQuantities("Cost", material.GetMaterialCost().GetQuantities());
            }
            else
            {
                infoPanel.AddValue(
                    "Production Cost", component.GetAttribute(ComponentAttribute.ProductionCost).ToString());
                infoPanel.AddQuantities(
                    "Cost",
                    component.MaterialCost.ToMultiQuantity(x => x.Key, x => x.Value.GetTotal()).GetQuantities());
            }
        }

        public void Describe(object @object, InfoPanel infoPanel)
        {
            DescribeAll(Enumerable.Repeat(@object, 1), infoPanel);
        }

        private void DescribeDefault(IEnumerable<IComponent> components, InfoPanel infoPanel)
        {
            foreach (var attribute in components.First().Attributes.Select(x => x.Key).OrderBy(x => x.ToString()))
            {
                if (IncludeBaseInfo || !s_BaseAttributes.Contains(attribute))
                {
                    infoPanel.AddValues(
                        StringUtils.FormatEnumString(attribute.ToString()),
                        components.Select(x => x.GetAttribute(attribute)), "{0:N0}");
                }
            }
        }
    }
}