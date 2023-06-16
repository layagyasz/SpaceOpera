using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.View;

namespace SpaceOpera.Controller
{
    public class UiInteractionEventArgs
    {
        public List<object> Objects { get; }
        public MouseButton? Button { get; }
        public Keys? Key { get; }
        public ActionId? Action { get; }

        private UiInteractionEventArgs(IEnumerable<object> objects, MouseButton? button, Keys? key, ActionId? action)
        {
            Objects = objects.ToList();
            Button = button;
            Key = key;
            Action = action;
        }

        public static UiInteractionEventArgs Create(object @object, MouseButton button)
        {
            return new(Enumerable.Repeat(@object, 1), button, null, null);
        }

        public static UiInteractionEventArgs Create<T>(IEnumerable<T> objects, MouseButton button)
        {
            return new(objects.Cast<object>(), button, null, null);
        }

        public static UiInteractionEventArgs Create(object @object, Keys key)
        {
            return new(Enumerable.Repeat(@object, 1), null, key, null);
        }

        public static UiInteractionEventArgs Create<T>(IEnumerable<T> objects, Keys key)
        {
            return new(objects.Cast<object>(), null, key, null);
        }

        public static UiInteractionEventArgs Create(object @object, ActionId action)
        {
            return new(Enumerable.Repeat(@object, 1), null, null, action);
        }

        public static UiInteractionEventArgs Create<T>(IEnumerable<T> objects, ActionId action)
        {
            return new(objects.Cast<object>(), null, null, action);
        }

        public object? GetOnlyObject()
        {
            return Objects.Count == 1 ? Objects[0] : null;
        }

        public UiInteractionEventArgs WithObject(object @object)
        {
            return WithObjects(Enumerable.Repeat(@object, 1));
        }

        public UiInteractionEventArgs WithObjects<T>(IEnumerable<T> objects)
        {
            return new(objects.Cast<object>(), Button, Key, Action);
        }

        public override string ToString()
        {
            return string.Format(
                "[UiInteractionEventArgs: Objects={0}, Button={1}, Key={2}, Action={3}]",
                string.Join(",", Objects),
                Button,
                Key,
                Action);
        }
    }
}
