namespace empireMaker
{
    /*
    // 생성시 퀘스트 방문객 pawn kind 제한
    internal class patch_QuestNode_GeneratePawn
    {
        
        [HarmonyPatch(typeof(QuestNode_GeneratePawn), "RunInt")]
        //[HarmonyPatch(new Type[] { typeof(Slate) })]
        public class patch
            {
            [HarmonyPrefix]
            public static bool Prefix(QuestNode_GeneratePawn __instance)
            {

                
                Slate slate = QuestGen.slate;
                Predicate<Pawn> predicate = null;
                PawnGenerationSpecialRequest? value = __instance.specialRequest.GetValue(slate);
                PawnGenerationSpecialRequest pawnGenerationSpecialRequest = PawnGenerationSpecialRequest.ExpertFighter;
                if (value.GetValueOrDefault() == pawnGenerationSpecialRequest & value != null)
                {
                    predicate = ((Pawn x) => x.skills.GetSkill(SkillDefOf.Shooting).Level >= 11 || x.skills.GetSkill(SkillDefOf.Melee).Level >= 11);
                }
                PawnKindDef value2 = __instance.kindDef.GetValue(slate);
                Faction value3 = __instance.faction.GetValue(slate);
                if (value2 != null)
                {
                    Log.Message("Wow : " + value2.defName);
                }
                else
                {
                    Log.Message("# pawnkind null");
                    
                }
                if(value3 != null) Log.Message(value3.def.defName);

                // yayo
                Faction faction = QuestGen.quest.InvolvedFactions.First<Faction>();
                value3 = faction;
                
                if (value2 != null && value2.defName == "SpaceRefugee_Clothed" && faction.def != FactionDefOf.Empire)
                {
                    value2 = (from x in DefDatabase<PawnKindDef>.AllDefs
                              where x.defaultFactionType == faction.def
                              select x).RandomElement<PawnKindDef>();
                              
                }
                if(value2 == null)
                {
                    Log.Message(faction.def.defName);
                    if (faction.def != FactionDefOf.Empire)
                    {
                        value2 = PawnKindDef.Named("SpaceRefugee_Clothed");
                    }
                    else
                    {
                        value2 = (from x in DefDatabase<PawnKindDef>.AllDefs
                                  where x.defaultFactionType == faction.def
                                  select x).RandomElement<PawnKindDef>();
                    }
                }
                //

                PawnGenerationContext context = PawnGenerationContext.NonPlayer;
                int tile = -1;
                bool forceGenerateNewPawn = false;
                bool newborn = false;
                bool allowDead = false;
                bool allowDowned = false;
                bool canGeneratePawnRelations = true;
                Predicate<Pawn> validatorPreGear = predicate;
                bool flag = __instance.allowAddictions.GetValue(slate) ?? true;
                IEnumerable<TraitDef> value4 = __instance.forcedTraits.GetValue(slate);
                float value5 = __instance.biocodeWeaponChance.GetValue(slate);
                Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(value2, value3, context, tile, forceGenerateNewPawn, newborn, allowDead, allowDowned, canGeneratePawnRelations, __instance.mustBeCapableOfViolence.GetValue(slate), 1f, false, true, true, flag, false, false, false, false, value5, __instance.extraPawnForExtraRelationChance.GetValue(slate), __instance.relationWithExtraPawnChanceFactor.GetValue(slate), validatorPreGear, null, value4, null, null, null, null, null, null, null, null, null));
                if(pawn == null)
                if (__instance.ensureNonNumericName.GetValue(slate) && (pawn.Name == null || pawn.Name.Numerical))
                {
                    pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, null);
                }
                if (__instance.storeAs.GetValue(slate) != null)
                {
                    QuestGen.slate.Set<Pawn>(__instance.storeAs.GetValue(slate), pawn, false);
                }
                if (__instance.addToList.GetValue(slate) != null)
                {
                    QuestGenUtility.AddToOrMakeList(QuestGen.slate, __instance.addToList.GetValue(slate), pawn);
                }
                QuestGen.AddToGeneratedPawns(pawn);
                if (!pawn.IsWorldPawn())
                {
                    Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
                }

                return false;






            }
        }
        

    }
    */
}