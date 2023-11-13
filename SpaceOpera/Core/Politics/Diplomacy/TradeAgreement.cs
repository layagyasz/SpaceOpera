using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Politics.Diplomacy
{
    public class TradeAgreement : IDiplomaticAgreementSection
    {
        public DiplomacyType Type => DiplomacyType.Trade;
        public Trade Trade { get; }

        public TradeAgreement(Trade trade)
        {
            Trade = trade;
        }

        public void Apply(World world, DiplomaticRelation relation)
        {
            world.Economy.AddTrade(Trade);
        }

        public void Cancel(World world, DiplomaticRelation relation)
        {
            world.Economy.RemoveTrade(Trade);
        }

        public void Notify(
            World world, DiplomaticRelation relation, IDiplomaticAgreementSection section, bool isProposer)
        { }

        public bool Validate(World world)
        {
            return Trade.FromZone.StellarBody == Trade.ToZone.StellarBody;
        }
    }
}
