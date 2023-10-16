using Cardamom.Utils.Suppliers;
using SpaceOpera.View.Forms;

namespace SpaceOpera.Controller
{
    public record class PopupEventArgs(FormLayout Layout, Promise<FormValue> Promise) { }
}
