using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld.QuestGen;
using RimWorld;

namespace empireMaker
{
    // 방문객 pawnkind 수정
    public class QuestNode_GetPawnKind : QuestNode
    {
        protected override bool TestRunInt(Slate slate)
        {
            this.SetVars(slate);
            return true;
        }

        protected override void RunInt()
        {
            this.SetVars(QuestGen.slate);
        }

        private void SetVars(Slate slate)
        {
            QuestNode_GetPawnKind.Option option = this.options.GetValue(slate).RandomElementByWeight((QuestNode_GetPawnKind.Option x) => x.weight);

            Pawn asker = this.pawn.GetValue(slate); // yayo
            
            PawnKindDef var;
            if (option.kindDef != null)
            {
                var = option.kindDef;
                // yayo

                if (var.defName == "SpaceRefugee_Clothed" && asker != null && asker.Faction != null && asker.Faction.def != FactionDefOf.Empire)
                {
                    var = PawnKindDef.Named($"SpaceRefugee_Clothed_{asker.Faction.def.defName}");

                }
                //
            }
            else if (option.anyAnimal)
            {
                var = (from x in DefDatabase<PawnKindDef>.AllDefs
                       where x.RaceProps.Animal && (option.onlyAllowedFleshType == null || x.RaceProps.FleshType == option.onlyAllowedFleshType)
                       select x).RandomElement<PawnKindDef>();
            }
            else
            {
                var = null;
            }
            slate.Set<PawnKindDef>(this.storeAs.GetValue(slate), var, false);
        }

        [NoTranslate]
        public SlateRef<string> storeAs;

        // Token: 0x04003D2E RID: 15662
        public SlateRef<List<QuestNode_GetPawnKind.Option>> options;

        public SlateRef<Pawn> pawn;


        // Token: 0x02001E2F RID: 7727
        public class Option
        {
            // Token: 0x040070D5 RID: 28885
            public PawnKindDef kindDef;

            // Token: 0x040070D6 RID: 28886
            public float weight;

            // Token: 0x040070D7 RID: 28887
            public bool anyAnimal;

            // Token: 0x040070D8 RID: 28888
            public FleshTypeDef onlyAllowedFleshType;
        }
    }
}
