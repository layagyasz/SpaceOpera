using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SpaceOpera.Controller
{
    public class UiInteractionEventArgs
    {
        public List<object> Objects { get; }
        public UiInteractionType Type { get; }

        private UiInteractionEventArgs(IEnumerable<object> Objects, UiInteractionType Type)
        {
            this.Objects = Objects.ToList();
            this.Type = Type;
        }

        public static UiInteractionEventArgs Create(object Object, UiInteractionType Type)
        {
            return new UiInteractionEventArgs(Enumerable.Repeat(Object, 1), Type);
        }

        public static UiInteractionEventArgs Create<T>(IEnumerable<T> Objects, UiInteractionType Type)
        {
            return new UiInteractionEventArgs(Objects.Cast<object>(), Type);
        }

        public static UiInteractionEventArgs Click(object Object, MouseButton button)
        {
            return Create(Object, ToInteractionType(button));
        }

        public override string ToString()
        {
            return string.Format(
                "[UiInteractionEventArgs: Objects={0}, Type={1}]",
                string.Join(",", Objects.Select(x => x.GetType())),
                Type);
        }

        private static UiInteractionType ToInteractionType(MouseButton button)
        {
            return button switch
            {
                MouseButton.Left => UiInteractionType.Click,
                MouseButton.Right => UiInteractionType.RightClick,
                _ => UiInteractionType.Unknown,
            };
        }
    }
}
