using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Panes;
using SpaceOpera.View.Panes.DesignPanes;

namespace SpaceOpera.Controller.Panes.DesignPanes
{
    public class DesignPaneController : MultiTabGamePaneController
    {
        public override void Bind(object @object)
        {
            base.Bind(@object);
            var pane = _pane as DesignPane;
            var tableController = (ActionTableController<Design>)pane!.DesignTable.ComponentController;
            tableController.Interacted += HandleInteraction;
            tableController.RowSelected += HandleDesignSelected;
        }

        public override void Unbind()
        {
            var pane = _pane as DesignPane;
            var tableController = (ActionTableController<Design>)pane!.DesignTable.ComponentController;
            tableController!.Interacted -= HandleInteraction;
            tableController.RowSelected -= HandleDesignSelected;
            base.Unbind();
        }

        private void HandleInteraction(object? @object, UiInteractionEventArgs e)
        {
            if (e.GetOnlyObject() is Type type)
            {
                if (type == typeof(Design))
                {
                    Interacted?.Invoke(this, e.WithObject(((MultiTabGamePane)_pane!).GetTab()));
                }
            }
            else
            {
                Interacted?.Invoke(this, e);
            }
        }

        private void HandleDesignSelected(object? sender, Design? design)
        {
            ((DesignPane)_pane!).SetInfo(design);
        }
    }
}
