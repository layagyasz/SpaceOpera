using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SpaceOpera.Controller
{
    public class UiInteractionEventArgs
    {
        public List<object> Objects { get; }
        public MouseButton? Button { get; }
        public Keys? Key { get; }

        private UiInteractionEventArgs(IEnumerable<object> objects, MouseButton? button, Keys? key)
        {
            Objects = objects.ToList();
            Button = button;
            Key = key;
        }

        public static UiInteractionEventArgs Create(object @object, MouseButton button)
        {
            return new(Enumerable.Repeat(@object, 1), button, null);
        }

        public static UiInteractionEventArgs Create<T>(IEnumerable<T> objects, MouseButton button)
        {
            return new(objects.Cast<object>(), button, null);
        }

        public static UiInteractionEventArgs Create(object @object, Keys key)
        {
            return new(Enumerable.Repeat(@object, 1), null, key);
        }

        public static UiInteractionEventArgs Create<T>(IEnumerable<T> objects, Keys key)
        {
            return new(objects.Cast<object>(), null, key);
        }

        public object? GetOnlyObject()
        {
            return Objects.Count == 1 ? Objects[0] : null;
        }

        public override string ToString()
        {
            return string.Format(
                "[UiInteractionEventArgs: Objects={0}, Button={1}, Key={2}]",
                string.Join(",", Objects.Select(x => x.GetType())),
                Button,
                Key);
        }
    }
}
