namespace empireMaker;


// 퀘스트 def를 읽어올 때 퀘스트 방문객 pawn kind 제한
/*
internal class patch_QuestNode_GetPawnKind
{
    [HarmonyPatch(typeof(RimWorld.QuestGen.QuestNode_GetPawnKind), "SetVars")]
    [HarmonyPatch(new Type[] { typeof(Slate) })]
    public class patch
        {
        [HarmonyPrefix]
        public static bool Prefix(RimWorld.QuestGen.QuestNode_GetPawnKind __instance, Slate slate)
        {
            RimWorld.QuestGen.QuestNode_GetPawnKind.Option option = __instance.options.GetValue(slate).RandomElementByWeight((RimWorld.QuestGen.QuestNode_GetPawnKind.Option x) => x.weight);

            Pawn asker;
            
            //Log.Message(QuestGen.quest.InvolvedFactions.First<Faction>().def.defName);
            //Log.Message(QuestGen.quest.InvolvedFactions.Count<Faction>().ToString());
            
            PawnKindDef var;
            if (option.kindDef != null)
            {
                var = option.kindDef;
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
            slate.Set<PawnKindDef>(__instance.storeAs.GetValue(slate), var, false);

            return false;






        }
    }


}
*/