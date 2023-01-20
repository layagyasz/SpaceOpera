namespace SpaceOpera.Core.Orders
{
    public enum ValidationFailureReason
    {
        None,
        IllegalOrder,
        TooFewResourceNodes,
        TooFewStructureNodes,
        TooFewStructures,
        PrerequisiteResearch,
        DuplicateResearch,
        InvalidDesign
    }
}
