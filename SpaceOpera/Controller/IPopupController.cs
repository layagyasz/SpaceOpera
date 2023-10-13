using SpaceOpera.View.Forms;

namespace SpaceOpera.Controller
{
    public interface IPopupController
    {
        EventHandler<FormLayout>? PopupCreated { get; set; }
    }
}
