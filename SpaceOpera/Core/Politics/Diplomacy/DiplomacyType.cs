using System.Collections.Immutable;

namespace SpaceOpera.Core.Politics.Diplomacy
{
    public record class DiplomacyType(bool IsMirrored, bool IsOriginator, bool IsUnilateral, bool IsUnique, int SetId)
    {
        public static readonly DiplomacyType DefensePact = 
            new(
                /* IsMirrored= */ true, 
                /* IsOriginator= */ false,
                /* IsUnilateral= */ false,
                /* IsUnique= */ true, 
                /* SetId= */ 0);

        public static readonly DiplomacyType Peace =
            new(
                /* IsMirrored= */ true,
                /* IsOriginator= */ true,
                /* IsUnilateral= */ false,
                /* IsUnique= */ true,
                /* SetId= */ 0);

        public static readonly DiplomacyType Trade =
            new(
                /* IsMirrored= */ false,
                /* IsOriginator= */ false,
                /* IsUnilateral= */ false,
                /* IsUnique= */ false,
                /* SetId= */ 0);

        public static readonly DiplomacyType War =
            new(
                /* IsMirrored= */ true,
                /* IsOriginator= */ true,
                /* IsUnilateral= */ true,
                /* IsUnique= */ true,
                /* SetId= */ 1);

        public static readonly ImmutableList<DiplomacyType> All = ImmutableList.Create(DefensePact, Peace, Trade, War);
    }
}
