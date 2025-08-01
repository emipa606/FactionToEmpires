using HarmonyLib;
using RimWorld;
using Verse;

namespace empireMaker;
// 초능력 관련 불법 체크 알고리즘 수정

[HarmonyPatch(typeof(ThingRequiringRoyalPermissionUtility),
    nameof(ThingRequiringRoyalPermissionUtility.IsViolatingRulesOf), typeof(Def), typeof(Pawn), typeof(Faction),
    typeof(int))]
public class ThingRequiringRoyalPermissionUtility_IsViolatingRulesOf
{
    [HarmonyPrefix]
    public static bool Prefix(ref bool __result, Def implantOrWeapon, Pawn pawn, Faction faction,
        int implantLevel = 0)
    {
        if (!EmpireMaker.phychicAll)
        {
            return true;
        }

        foreach (var faction2 in Find.FactionManager.AllFactionsListForReading)
        {
            if (faction2 == faction ||
                EmpireMaker.IsViolatingRulesOf(implantOrWeapon, pawn, faction2, implantLevel))
            {
                continue;
            }

            __result = false;
            return false;
        }

        if (faction.def.royalImplantRules == null || faction.def.royalImplantRules.Count == 0)
        {
            __result = false;
            return false;
        }

        var minTitleToUse =
            ThingRequiringRoyalPermissionUtility.GetMinTitleToUse(implantOrWeapon, faction, implantLevel);
        if (minTitleToUse == null)
        {
            __result = false;
            return false;
        }

        var currentTitle = pawn.royalty.GetCurrentTitle(faction);
        if (currentTitle == null)
        {
            __result = true;
            return false;
        }

        var num = faction.def.RoyalTitlesAwardableInSeniorityOrderForReading.IndexOf(currentTitle);
        if (num < 0)
        {
            __result = false;
            return false;
        }

        var num2 = faction.def.RoyalTitlesAwardableInSeniorityOrderForReading.IndexOf(minTitleToUse);
        __result = num < num2;
        return false;
    }
}