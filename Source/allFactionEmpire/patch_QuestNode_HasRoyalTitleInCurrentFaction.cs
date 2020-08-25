using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using RimWorld.QuestGen;
using RimWorld;
using Verse;
using UnityEngine;
using RimWorld.Planet;

namespace empireMaker
{
    // 퀘스트 asker 종족, 팩션제한
    /*
    internal class patch_QuestNode_HasRoyalTitleInCurrentFaction
    {
        [HarmonyPatch(typeof(QuestNode_HasRoyalTitleInCurrentFaction), "HasRoyalTitleInCurrentFaction")]
        [HarmonyPatch(new Type[] { typeof(Slate) })]
        public class patch
            {
            [HarmonyPrefix]
            public static bool Prefix(ref bool __result, QuestNode_HasRoyalTitleInCurrentFaction __instance, Slate slate)
            {

                Pawn value = __instance.pawn.GetValue(slate);
                __result = value != null && value.Faction != null && value.royalty != null && value.royalty.HasAnyTitleIn(value.Faction) && (value.Faction == Faction.Empire || (from x in DefDatabase<PawnKindDef>.AllDefs
                                                                                                                                             where x.defaultFactionType == value.Faction.def
                                                                                                                                             select x).Contains(value.kindDef));
                return false;


            }
        }


    }
    */
    
    


}
