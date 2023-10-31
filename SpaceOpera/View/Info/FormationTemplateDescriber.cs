using SpaceOpera.Core.Military;

namespace SpaceOpera.View.Info
{
    public class FormationTemplateDescriber : IDescriber
    {
        public void DescribeAll(IEnumerable<object> objects, InfoPanel infoPanel)
        {
            Describe(objects.First(), infoPanel);
        }

        public void Describe(object @object, InfoPanel infoPanel)
        {
            var formation = (IFormationTemplate)@object;
            foreach (var unit in formation.Composition)
            {
                infoPanel.AddValue(unit.Key.Name, string.Format("{0}", unit.Value));
            }
        }
    }
}
