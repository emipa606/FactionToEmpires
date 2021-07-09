namespace empireMaker
{
    // 캐러밴 공물 징수원 수정
    /*
    public class IncidentWorker_CaravanArrivalTributeCollector : IncidentWorker_TraderCaravanArrival
    {
        // Token: 0x06003B7A RID: 15226 RVA: 0x00139010 File Offset: 0x00137210
        protected override bool TryResolveParmsGeneral(IncidentParms parms)
        {
            if (!TryResolveParmsGeneral(parms))
            {
                return false;
            }
            Map map = (Map)parms.target;
            parms.faction = Faction.Empire;
            parms.traderKind = (from t in DefDatabase<TraderKindDef>.AllDefsListForReading
                                where t.category == "TributeCollector"
                                select t).RandomElementByWeight((TraderKindDef t) => this.TraderKindCommonality(t, map, parms.faction));
            return true;
        }

        // Token: 0x06003B7B RID: 15227 RVA: 0x001390B0 File Offset: 0x001372B0
        protected override void SendLetter(IncidentParms parms, List<Pawn> pawns, TraderKindDef traderKind)
        {
            TaggedString baseLetterLabel = "LetterLabelTributeCollectorArrival".Translate().CapitalizeFirst();
            TaggedString taggedString = "LetterTributeCollectorArrival".Translate(parms.faction.Named("FACTION")).CapitalizeFirst();
            taggedString += "\n\n" + "LetterCaravanArrivalCommonWarning".Translate();
            PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(pawns, ref baseLetterLabel, ref taggedString, "LetterRelatedPawnsNeutralGroup".Translate(Faction.OfPlayer.def.pawnsPlural), true, true);
            base.SendStandardLetter(baseLetterLabel, taggedString, LetterDefOf.PositiveEvent, parms, pawns[0], Array.Empty<NamedArgument>());
        }

        // Token: 0x040022B9 RID: 8889
        public const string TributeCollectorTraderKindCategory = "TributeCollector";
    }
    */
}