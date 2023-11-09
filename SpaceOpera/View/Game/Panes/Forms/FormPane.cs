using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Utils.Suppliers;
using SpaceOpera.Controller.Game.Panes.Forms;
using SpaceOpera.Core;
using SpaceOpera.View.Components.NumericInputs;
using SpaceOpera.View.Forms;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.Forms
{
    public class FormPane : SimpleGamePane
    {
        private static readonly string s_Container = "form-pane";
        private static readonly string s_Title = "form-pane-title";
        private static readonly string s_Close = "form-pane-close";
        private static readonly string s_Body = "form-pane-body";
        private static readonly string s_Submit = "form-pane-submit";

        private static readonly Form.Style s_Style =
            new()
            {
                Container = "form-pane-form-container",
                Paragraph = "form-pane-paragraph",
                Header3 = "form-pane-form-field-header",
                Info =
                    new()
                    {
                        Container = "form-pane-info-container",
                        Row = "form-pane-info-row",
                        RowHeading = "form-pane-info-heading",
                        RowValue = "form-pane-info-value",
                        MaterialCell = "form-pane-info-material-cell",
                        MaterialIcon = "form-pane-info-material-icon",
                        MaterialText = "form-pane-info-material-text"
                    },
                DropDown =
                    new()
                    {
                        Root = "form-pane-select",
                        OptionContainer = "form-pane-option-container",
                        Option = "form-pane-option"
                    },
                Dial =
                    new()
                    {
                        Container = "form-pane-dial",
                        Text = "form-pane-dial-text",
                        LeftButton = "form-pane-dial-left-button",
                        RightButton = "form-pane-dial-right-button"
                    },
                Radio = 
                    new()
                    {
                        Container = "form-pane-radio",
                        Option = "form-pane-radio-option"
                    },
                MultiCount =
                    new()
                    {
                        Container = "form-pane-multi-count",
                        Table = "form-pane-multi-count-table",
                        SelectWrapper = "form-pane-multi-count-select-wrapper",
                        Select = 
                            new()
                            {
                                Root = "form-pane-multi-count-select",
                                OptionContainer = "form-pane-multi-count-select-option-container",
                                Option = "form-pane-multi-count-select-option"
                            },
                        Add = "form-pane-multi-count-add",
                        Row = 
                            new MultiCountInputStyles.ManualMultiCountInputRowStyle()
                            {
                                Container = "form-pane-multi-count-row",
                                Info = "form-pane-multi-count-row-info",
                                Icon = "form-pane-multi-count-row-icon",
                                Text = "form-pane-multi-count-row-text",
                                NumericInput = new()
                                {
                                    Container = "form-pane-multi-count-row-numeric-input",
                                    Text = "form-pane-multi-count-row-numeric-input-text",
                                    SubtractButton = "form-pane-multi-count-row-numeric-input-subtract",
                                    AddButton = "form-pane-multi-count-row-numeric-input-add"
                                },
                                Remove = "form-pane-multi-count-row-remove"
                            },
                        TotalContainer = "form-pane-multi-count-total-row",
                        TotalText = "form-pane-multi-count-total-text",
                        TotalNumber = "form-pane-multi-count-total-number",
                    }
            };

        public UiSerialContainer Contents { get; }
        public IUiElement Submit { get; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private World? _world;
        private Form? _form;
        private Promise<FormValue>? _promise;

        public FormPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new FormPaneController(),
                  uiElementFactory.GetClass(s_Container),
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), string.Empty),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1)
        {
            Submit = uiElementFactory.CreateTextButton(s_Submit, "Submit").Item1;
            Contents =
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_Body),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Vertical)
                {
                    Submit
                };
            SetBody(Contents);
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
        }

        public Form GetForm()
        {
            return _form!;
        }

        public Promise<FormValue> GetPromise()
        {
            return _promise!;
        }

        public override void Populate(params object?[] args)
        {
            if (_form != null)
            {
                Contents.Remove(_form, /* dispose= */ true);
            }

            _world = (World)args[0]!;
            var layout = (FormLayout)args[1]!;
            _form = (Form)layout.Create(s_Style, _uiElementFactory, _iconFactory);
            _form.Initialize();
            _promise = (Promise<FormValue>)args[2]!;
            Contents.Insert(0, _form);
            Submit.Visible = !_form.AutoSubmit;
            SetTitle(_form.Name);
            Populated?.Invoke(this, EventArgs.Empty);
        }
    }
}
