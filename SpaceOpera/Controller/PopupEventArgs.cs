using Cardamom.Utils.Suppliers.Promises;
using SpaceOpera.View.Forms;

namespace SpaceOpera.Controller
{
    public record class PopupEventArgs(FormLayout Layout, IPromise<FormValue> Promise) { }
}
