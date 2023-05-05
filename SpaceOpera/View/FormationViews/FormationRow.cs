using Cardamom.Collections;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.FormationsViews;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.FormationViews
{
    public class FormationRow : DynamicUiCompoundComponent, IActionRow
    {
        private static readonly string s_FormationLayerRow = "formation-layer-row";
        private static readonly string s_FormationLayerRowIcon = "formation-layer-row-icon";
        private static readonly string s_FormationLayerRowText = "formation-layer-row-text";
        private static readonly string s_FormationLayerRowNumber = "formation-layer-row-number";
        private static readonly string s_FormationLayerRowCombatIcon = "formation-layer-row-combat-icon";
        private static readonly string s_FormationLayerRowMoveIcon = "formation-layer-row-move-icon";
        private static readonly string s_FormationLayerRowRegroupIcon = "formation-layer-row-regroup-icon";
        private static readonly string s_FormationLayerRowSpotIcon = "formation-layer-row-spot-icon";
        private static readonly string s_FormationLayerRowTrainIcon = "formation-layer-row-train-icon";

        public EventHandler<ElementEventArgs>? ActionAdded { get; set; }
        public EventHandler<ElementEventArgs>? ActionRemoved { get; set; }

        public int FormationCount => _drivers.Count;

        private readonly UiElementFactory _uiElementFactory;
        private readonly List<AtomicFormationDriver> _drivers = new();
        private readonly TextUiElement _number;

        private ActionType _actionType;
        private IUiElement? _action;

        public FormationRow(object key, string name, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new FormationRowController(), 
                  new UiSerialContainer(
                      uiElementFactory.GetClass(s_FormationLayerRow),
                      new ButtonController(),
                      UiSerialContainer.Orientation.Horizontal))
        {
            _uiElementFactory = uiElementFactory;
            _number = 
                new TextUiElement(
                    uiElementFactory.GetClass(s_FormationLayerRowNumber), new InlayController(), string.Empty);

            Add(
                iconFactory.Create(
                    uiElementFactory.GetClass(s_FormationLayerRowIcon),
                    new InlayController(),
                    key));
            Add(
                new TextUiElement(
                    uiElementFactory.GetClass(s_FormationLayerRowText), new InlayController(), name));
            Add(_number);
        }

        public void Add(AtomicFormationDriver driver)
        {
            _drivers.Add(driver);
            UpdateNumber();
            Refresh();
        }

        public void Remove(AtomicFormationDriver driver)
        {
            _drivers.Remove(driver);
            UpdateNumber();
            Refresh();
        }

        public IEnumerable<IUiElement> GetActions() 
        {
            return Enumerable.Empty<IUiElement>();
        }

        public IEnumerable<AtomicFormationDriver> GetDrivers()
        {
            return _drivers;
        }

        public override void Refresh()
        {
            var newActionType = 
                _drivers.Select(x => x.GetCurrentAction()?.Type ?? ActionType.Unknown).ArgMax(GetValue);
            if (_actionType != newActionType)
            {
                if (_action != null)
                {
                    Remove(_action);
                    ActionRemoved?.Invoke(this, new(_action));
                }
                var @class = GetClass(newActionType);
                if (@class != null)
                {
                    _action = 
                        new SimpleUiElement(
                            _uiElementFactory.GetClass(@class), new ActionButtonController(GetAction(newActionType)));
                    _action.Initialize();
                    Add(_action);
                    ActionAdded?.Invoke(this, new(_action));
                }
                _actionType = newActionType;
            }
            base.Refresh();
        }

        private void UpdateNumber()
        {
            if (FormationCount > 1)
            {
                _number.SetText(FormationCount.ToString());
            }
            else
            {
                _number.SetText(string.Empty);
            }
        }

        private static float GetValue(ActionType actionType)
        {
            return actionType switch
            {
                ActionType.Unknown => 0,
                ActionType.Combat => 5,
                ActionType.Move => 3,
                ActionType.None => 0,
                ActionType.Regroup => 1,
                ActionType.Spot => 4,
                ActionType.Train => 2,
                _ => 0,
            };
        }

        private static string? GetClass(ActionType actionType)
        {
            return actionType switch
            {
                ActionType.Combat => s_FormationLayerRowCombatIcon,
                ActionType.Move => s_FormationLayerRowMoveIcon,
                ActionType.Regroup => s_FormationLayerRowRegroupIcon,
                ActionType.Spot => s_FormationLayerRowSpotIcon,
                ActionType.Train => s_FormationLayerRowTrainIcon,
                _ => null,
            };
        }

        private static ActionId GetAction(ActionType actionType)
        {
            return actionType switch
            {
                ActionType.Combat => ActionId.Battle,
                _ => ActionId.Select
            };
        }
    }
}
