using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;

namespace SpaceOpera.View.Panes
{
    public class TabBar<T>
    {
        public struct Definition
        {
            public T Key { get; set; }
            public string Text { get; set; }

            public Definition(T key, string text)
            {
                Key = key;
                Text = text;
            }
        }

        public static UiComponent Create(
            IEnumerable<Definition> definitions, Class containerClass, Class tabOptionClass)
        {
            var container = 
                new UiSerialContainer(
                    containerClass, new ButtonController(), UiSerialContainer.Orientation.Horizontal);
            foreach (var definition in definitions)
            {
                container.Add(
                    new TextUiElement(tabOptionClass, new OptionController<object>(definition.Key!), definition.Text));
            }
            return new UiComponent(new RadioController<object>("tab-bar"), container);
        }
    }
}
