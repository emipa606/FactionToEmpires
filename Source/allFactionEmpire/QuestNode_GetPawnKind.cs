using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace empireMaker
{
    // 방문객 pawnkind 수정
    public class QuestNode_GetPawnKind : QuestNode
    {
        // Token: 0x04003D2E RID: 15662
        public SlateRef<List<Option>> options;

        public SlateRef<Pawn> pawn;

        [NoTranslate] public SlateRef<string> storeAs;

        protected override bool TestRunInt(Slate slate)
        {
            SetVars(slate);
            return true;
        }

        protected override void RunInt()
        {
            SetVars(QuestGen.slate);
        }

        private void SetVars(Slate slate)
        {
            var option = options.GetValue(slate).RandomElementByWeight(x => x.weight);

            var asker = pawn.GetValue(slate); // yayo

            PawnKindDef var;
            if (option.kindDef != null)
            {
                var = option.kindDef;
                // yayo

                if (var.defName == "SpaceRefugee_Clothed" && asker?.Faction != null &&
                    asker.Faction.def != FactionDefOf.Empire)
                {
                    var = PawnKindDef.Named($"SpaceRefugee_Clothed_{asker.Faction.def.defName}");
                }

                //
            }
            else if (option.anyAnimal)
            {
                var = (from x in DefDatabase<PawnKindDef>.AllDefs
                    where x.RaceProps.Animal && (option.onlyAllowedFleshType == null ||
                                                 x.RaceProps.FleshType == option.onlyAllowedFleshType)
                    select x).RandomElement();
            }
            else
            {
                var = null;
            }

            slate.Set(storeAs.GetValue(slate), var);
        }


        // Token: 0x02001E2F RID: 7727
        public class Option
        {
            // Token: 0x040070D7 RID: 28887
            public bool anyAnimal;

            // Token: 0x040070D5 RID: 28885
            public PawnKindDef kindDef;

            // Token: 0x040070D8 RID: 28888
            public FleshTypeDef onlyAllowedFleshType;

            // Token: 0x040070D6 RID: 28886
            public float weight;
        }
    }
}