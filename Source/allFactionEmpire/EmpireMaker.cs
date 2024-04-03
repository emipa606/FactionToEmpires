using System.Collections.Generic;
using System.Linq;
using HugsLib;
using HugsLib.Settings;
using RimWorld;
using UnityEngine;
using Verse;

namespace empireMaker;

public class EmpireMaker : ModBase
{
    public enum ApparelType
    {
        off,
        forced,
        basic
    }

    public enum MakeType
    {
        no,
        empire,
        bugFix
    }

    public enum RelationType
    {
        basic,
        empire,
        ally,
        neutral,
        enemy,
        permanentEnemy
    }

    public static bool phychicAll = true;
    public static bool delVanilla;
    public static float questAmount = 1f;
    public static bool debugMode;

    public static readonly List<FactionDef> factionDefList = [];
    public static readonly List<MakeType> factionToEmpireList = [];
    public static readonly List<SettingHandle<RelationType>> relationSettingList = [];
    public static readonly List<RelationType> relationList = [];
    public static readonly List<ApparelType> apparelList = [];
    public static List<bool> collectorList = [];
    public static readonly List<bool> tradePermitList = [];
    public static List<FactionDef> willBeEmpireFactionDefList = [];


    // 비호환 하드모드 패키지 아이디
    public static readonly List<string> hardModsToConvert = ["projectjedi.factions", "kikohi.forsakens"];

    private SettingHandle<bool> debugModeSetting;

    private SettingHandle<bool> delVanillaSetting;

    private SettingHandle<bool> phychicAllSetting;

    private SettingHandle<float> questAmountSetting;

    public override string ModIdentifier => "empireMaker";

    public override void DefsLoaded()
    {
        //Setup and get settings values
        foreach (var faction in from faction in DefDatabase<FactionDef>.AllDefs
                 where
                     faction.royalFavorLabel == null &&
                     !faction.hidden &&
                     //(faction.naturalColonyGoodwill.min >= 0 || faction.naturalColonyGoodwill.max >= 0) &&
                     !faction.mustStartOneEnemy &&
                     //!faction.permanentEnemy &&
                     faction.pawnGroupMakers != null &&
                     !faction.isPlayer
                 select faction
                )
        {
            factionDefList.Add(faction);
        }


        // 초능력 공용화
        phychicAllSetting =
            Settings.GetHandle("phychicAll", "phychicAll_t".Translate(), "phychicAll_d".Translate(), true);
        phychicAll = phychicAllSetting.Value;

        // 바닐라 제국 숨기기
        delVanillaSetting =
            Settings.GetHandle<bool>("delVanilla", "delVanilla_t".Translate(), "delVanilla_d".Translate());
        delVanillaSetting.NeverVisible = true;
        delVanilla = delVanillaSetting.Value;

        // 퀘스트 생성량
        questAmountSetting = Settings.GetHandle("questAmount", "questAmount_t".Translate(),
            "questAmount_d".Translate(), 1f);
        questAmountSetting.NeverVisible = true;
        questAmount = questAmountSetting.Value;

        for (var i = 0; i < factionDefList.Count; i++)
        {
            // 옵션 관리
            var factionDef = factionDefList[i];
            var isHardMod = hardModsToConvert.Contains(factionDef.modContentPack.PackageId);


            // 제국화
            if (factionDef.permanentEnemy)
            {
                factionToEmpireList.Add(Settings.GetHandle($"{factionDef.defName}ToEmpire",
                        $"==== {factionDef.label.ToUpper()} ====", "make_d".Translate(), MakeType.no, null,
                        "en_make_")
                    .Value);
            }
            else if (!isHardMod)
            {
                factionToEmpireList.Add(Settings.GetHandle($"{factionDef.defName}ToEmpire",
                    $"==== {factionDef.label.ToUpper()} ====", "make_d".Translate(), MakeType.empire, null,
                    "en_make_").Value);
            }
            else
            {
                // 이 모드가 하드모드 일경우에 bugFix로 설정
                var tmp = Settings.GetHandle($"{factionDef.defName}ToEmpire",
                    $"==== {factionDef.label.ToUpper()} ====", "make_d".Translate(), MakeType.bugFix, null,
                    "en_make_").Value;
                if (tmp == MakeType.empire)
                {
                    tmp = MakeType.bugFix;
                }

                factionToEmpireList.Add(tmp);
            }

            // 관계
            relationSettingList.Add(Settings.GetHandle($"{factionDef.defName}Relation", "relation_t".Translate(),
                "relation_d".Translate(), RelationType.basic, null, "en_relation_"));
            relationList.Add(relationSettingList[i].Value);
            //ar_faction_relation.Add(Settings.GetHandle<en_relation>($"{f.defName}Relation", $"relation_t".Translate(), "relation_d".Translate(), en_relation.basic, null, "en_relation_").Value);


            // 왕족 의상
            apparelList.Add(Settings.GetHandle($"{factionDef.defName}Apparel", "apparel_t".Translate(),
                "apparel_d".Translate(), ApparelType.basic, null, "en_apparel_").Value);


            // 거래제한과 권한
            tradePermitList.Add(Settings.GetHandle<bool>($"{factionDef.defName}TradePermit", "trade_t".Translate(),
                "trade_d".Translate()).Value);

            // 공물 징수원
            //ar_faction_collector.Add(Settings.GetHandle<bool>($"{f.defName}Collector", $"     - (not developed yet)use tribute collector", "A caravan trader selling royal favors.\n\n* Need to restart and new game", true).Value);
        }

        // 디버그 모드
        debugModeSetting =
            Settings.GetHandle<bool>("debugMode", "debugMode_t".Translate(), "debugMode_d".Translate());
        debugMode = debugModeSetting.Value;


        PatchDef();
    }


    public override void SettingsChanged()
    {
        phychicAll = phychicAllSetting.Value;
        delVanilla = delVanillaSetting.Value;

        switch (questAmountSetting.Value)
        {
            case < 0.1f:
                questAmountSetting.Value = 0.1f;
                break;
            case > 20f:
                questAmountSetting.Value = 20f;
                break;
        }

        questAmount = questAmountSetting.Value;

        debugMode = debugModeSetting.Value;
        for (var i = 0; i < relationList.Count; i++)
        {
            relationList[i] = relationSettingList[i].Value;
        }
    }

    public static void PatchDef()
    {
        // 제국
        var empireFactionDef = FactionDefOf.Empire;

        // 제국 타이틀
        var royalTitleDefList = new List<RoyalTitleDef>();

        foreach (var title in from titles in DefDatabase<RoyalTitleDef>.AllDefs
                 where
                     titles.tags.Contains("EmpireTitle")
                 select titles
                )
        {
            royalTitleDefList.Add(title);
        }

        royalTitleDefList.SortBy(t => t.favorCost);


        // 제국 폰 리스트
        var royalPawnKindDefList = new List<PawnKindDef>
        {
            PawnKindDef.Named("Empire_Royal_Yeoman"),
            PawnKindDef.Named("Empire_Royal_Acolyte"),
            PawnKindDef.Named("Empire_Royal_Knight"),
            PawnKindDef.Named("Empire_Royal_Praetor"),
            PawnKindDef.Named("Empire_Royal_Baron"),
            PawnKindDef.Named("Empire_Royal_Count"),
            PawnKindDef.Named("Empire_Royal_Duke"),
            PawnKindDef.Named("Empire_Royal_Consul"),
            PawnKindDef.Named("Empire_Royal_Stellarch")
        };


        for (var factionDef = 0; factionDef < factionDefList.Count; factionDef++)
        {
            var faction = factionDefList[factionDef];

            Log.Message(
                factionToEmpireList[factionDef] != MakeType.no
                    ? $"# make to empire : {factionDefList[factionDef].defName} ({factionDefList[factionDef].modContentPack.PackageId})"
                    : $"# not make to empire : {factionDefList[factionDef].defName} ({factionDefList[factionDef].modContentPack.PackageId})");

            if (factionToEmpireList[factionDef] == MakeType.no)
            {
                continue;
            }

            var useApparel = apparelList[factionDef];

            if (!faction.humanlikeFaction)
            {
                useApparel = ApparelType.off;
            }

            var bugFixMode = factionToEmpireList[factionDef] == MakeType.bugFix;
            //bool useCollectorTrader = ar_faction_collector[z];
            var useTradePermit = tradePermitList[factionDef];

            var permitPawns = new List<PawnKindDef>();
            var allPawns = new List<PawnKindDef>();
            var allPawnsNoLeaders = new List<PawnKindDef>();
            var allFighterPawns = new List<PawnKindDef>();
            var allPawnsLeaders = new List<PawnKindDef>();


            if (debugMode)
            {
                Log.Message("A");
            }


            // 팩션 제국화
            faction.royalFavorLabel = empireFactionDef.royalFavorLabel;
            faction.royalFavorIconPath = empireFactionDef.royalFavorIconPath;
            faction.raidLootMaker = empireFactionDef.raidLootMaker;

            faction.royalTitleInheritanceRelations = empireFactionDef.royalTitleInheritanceRelations;
            faction.royalTitleInheritanceWorkerClass = empireFactionDef.royalTitleInheritanceWorkerClass;
            //faction.minTitleForBladelinkWeapons = baseF.minTitleForBladelinkWeapons;
            var customTitleTag = $"{faction.defName}Title";
            faction.royalTitleTags = [customTitleTag];

            if (faction.colorSpectrum == null)
            {
                faction.colorSpectrum = [Color.white];
            }

            if (debugMode)
            {
                Log.Message("B");
            }

            foreach (var g in faction.pawnGroupMakers)
            {
                if (debugMode)
                {
                    Log.Message(g.options.Count.ToString());
                }

                foreach (var pawnGenOption in g.options)
                {
                    if (debugMode)
                    {
                        Log.Message("b");
                    }

                    var p = pawnGenOption.kind;
                    if (allPawns.Contains(p))
                    {
                        continue;
                    }

                    if (debugMode)
                    {
                        Log.Message($" - {faction.defName} : pawnkind : {p.defName}");
                    }

                    allPawns.Add(p);
                    if (p.factionLeader)
                    {
                        allPawnsLeaders.Add(p);
                    }
                    else
                    {
                        allPawnsNoLeaders.Add(p);
                    }
                }
            }

            allPawns = allPawns.OrderBy(a => a.combatPower).ToList();
            if (allPawnsNoLeaders.Count <= 0)
            {
                allPawnsNoLeaders = allPawns;
            }

            allPawnsNoLeaders = allPawnsNoLeaders.OrderBy(a => a.combatPower).ToList();
            allPawnsLeaders = allPawnsLeaders.OrderBy(a => a.combatPower).ToList();
            Log.Message(
                $" - {faction.defName} : total pawn count : {allPawns.Count}, leader count : {allPawnsLeaders.Count}");

            if (debugMode)
            {
                Log.Message("B1");
            }

            // fighter pawn 리스트
            foreach (var pawn in from pawn in allPawns
                     where
                         pawn.isFighter
                     select pawn)
            {
                allFighterPawns.Add(pawn);
            }

            allFighterPawns = allFighterPawns.OrderBy(a => a.combatPower).ToList();


            if (allFighterPawns.Count == 0)
            {
                allFighterPawns = allPawns;
                Log.Message(
                    $" - {faction.defName} : total fighter pawn count : {allFighterPawns.Count}, change to use allPawn");
            }
            else
            {
                Log.Message($" - {faction.defName} : total fighter pawn count : {allFighterPawns.Count}");
            }

            // 폰카인드 없음 스킵
            if (allPawns.Count == 0)
            {
                Log.Error(
                    $" - {faction.defName} : Can not read any pawnkind, Turn off the 'make empire' for this faction in the 'faction to empire mod' option.\nTurn off the 'make empire' for this faction in the 'faction to empire mod' option.\nAnd let the author know that this mod is not compatible.");
            }

            if (debugMode)
            {
                Log.Message("B2");
            }

            // 호위병 소환을 위한 폰 등록

            switch (allFighterPawns.Count)
            {
                case 0:
                    permitPawns.Add(PawnKindDef.Named("Empire_Fighter_Trooper"));
                    permitPawns.Add(PawnKindDef.Named("Empire_Fighter_Janissary"));
                    permitPawns.Add(PawnKindDef.Named("Empire_Fighter_Cataphract"));
                    break;
                case <= 3:
                {
                    for (var i = 0; i < 3; i++)
                    {
                        permitPawns.Add(allFighterPawns[Mathf.Clamp(i, 0, allFighterPawns.Count - 1)]);
                    }

                    break;
                }
                default:
                    permitPawns.Add(allFighterPawns[0]);
                    permitPawns.Add(allFighterPawns[Mathf.RoundToInt((allFighterPawns.Count - 1) * 0.5f)]);
                    permitPawns.Add(allFighterPawns[allFighterPawns.Count - 1]);
                    break;
            }

            if (debugMode)
            {
                Log.Message("C");
            }


            // 커스텀 권한 생성

            var tradeSettlementPermit = new RoyalTitlePermitDef();
            var tradeOrbitalPermit = new RoyalTitlePermitDef();
            var tradeCaravanPermit = new RoyalTitlePermitDef();

            var callMilitaryAidSmall = new RoyalTitlePermitDef();
            var callMilitaryAidLarge = new RoyalTitlePermitDef();
            var callMilitaryAidGrand = new RoyalTitlePermitDef();

            foreach (var royalTitlePermitDef in from permits in DefDatabase<RoyalTitlePermitDef>.AllDefs
                     where true
                     select permits)
            {
                RoyalTitlePermitDef newPermit = null;
                var n = 0;
                var totalCombatPower = 0f;
                // 복제
                switch (royalTitlePermitDef.defName)
                {
                    case "TradeSettlement":
                        tradeSettlementPermit.defName = $"TradeSettlement_{faction}";
                        tradeSettlementPermit.label = royalTitlePermitDef.label;
                        break;
                    case "TradeOrbital":
                        tradeOrbitalPermit.defName = $"TradeOrbital_{faction}";
                        tradeOrbitalPermit.label = royalTitlePermitDef.label;
                        break;
                    case "TradeCaravan":
                        tradeCaravanPermit.defName = $"TradeCaravan_{faction}";
                        tradeCaravanPermit.label = royalTitlePermitDef.label;
                        break;

                    case "CallMilitaryAidSmall":
                        newPermit = callMilitaryAidSmall;
                        n = 0;
                        totalCombatPower = 240f;
                        break;
                    case "CallMilitaryAidLarge":
                        newPermit = callMilitaryAidLarge;
                        n = 1;
                        totalCombatPower = 400f;
                        break;
                    case "CallMilitaryAidGrand":
                        newPermit = callMilitaryAidGrand;
                        n = 2;
                        totalCombatPower = 600f;
                        break;
                }

                if (newPermit == null)
                {
                    continue;
                }

                newPermit.defName = $"{royalTitlePermitDef.defName}_{faction.defName}";
                newPermit.label = royalTitlePermitDef.label;
                newPermit.workerClass = royalTitlePermitDef.workerClass;
                newPermit.cooldownDays = royalTitlePermitDef.cooldownDays;
                newPermit.royalAid = new RoyalAid
                {
                    favorCost = royalTitlePermitDef.royalAid.favorCost,
                    pawnKindDef = permitPawns[n],
                    pawnCount = Mathf.RoundToInt(totalCombatPower / permitPawns[n].combatPower)
                };
            }

            DefDatabase<RoyalTitlePermitDef>.Add(tradeSettlementPermit);
            DefDatabase<RoyalTitlePermitDef>.Add(tradeOrbitalPermit);
            DefDatabase<RoyalTitlePermitDef>.Add(tradeCaravanPermit);

            DefDatabase<RoyalTitlePermitDef>.Add(callMilitaryAidSmall);
            DefDatabase<RoyalTitlePermitDef>.Add(callMilitaryAidLarge);
            DefDatabase<RoyalTitlePermitDef>.Add(callMilitaryAidGrand);


            if (debugMode)
            {
                Log.Message("D");
            }


            // 커스텀 작위
            var newRoyalTitleDefList = new List<RoyalTitleDef>();
            var newRoyalTitleDefListNoStella = new List<RoyalTitleDef>();
            var newRoyalTitleDefListNoEmperor = new List<RoyalTitleDef>();
            for (var i = 0; i < royalTitleDefList.Count; i++)
            {
                // 복제
                var royalTitleDef = royalTitleDefList[i];
                var newRoyalTitleDef = new RoyalTitleDef
                {
                    defName = $"{royalTitleDef.defName}_{faction.defName}",
                    tags = []
                };
                newRoyalTitleDef.tags.AddRange(faction.royalTitleTags);

                newRoyalTitleDef.awardThought = royalTitleDef.awardThought;
                newRoyalTitleDef.bedroomRequirements = royalTitleDef.bedroomRequirements;
                newRoyalTitleDef.changeHeirQuestPoints = royalTitleDef.changeHeirQuestPoints;
                newRoyalTitleDef.commonality = royalTitleDef.commonality;
                newRoyalTitleDef.debugRandomId = royalTitleDef.debugRandomId; // 디버그 랜덤아이디?
                newRoyalTitleDef.decreeMentalBreakCommonality = royalTitleDef.decreeMentalBreakCommonality;
                newRoyalTitleDef.decreeMinIntervalDays = royalTitleDef.decreeMinIntervalDays;
                newRoyalTitleDef.decreeMtbDays = royalTitleDef.decreeMtbDays;
                newRoyalTitleDef.decreeTags = royalTitleDef.decreeTags;
                newRoyalTitleDef.description = royalTitleDef.description;
                newRoyalTitleDef.descriptionHyperlinks = royalTitleDef.descriptionHyperlinks;
                newRoyalTitleDef.disabledJoyKinds = royalTitleDef.disabledJoyKinds;
                newRoyalTitleDef.disabledWorkTags = royalTitleDef.disabledWorkTags;
                newRoyalTitleDef.favorCost = royalTitleDef.favorCost;
                newRoyalTitleDef.fileName = royalTitleDef.fileName; // 파일명?
                newRoyalTitleDef.foodRequirement = royalTitleDef.foodRequirement;
                newRoyalTitleDef.generated = royalTitleDef.generated; // 생성완료?
                newRoyalTitleDef.ignoreConfigErrors = royalTitleDef.ignoreConfigErrors;
                newRoyalTitleDef.index = royalTitleDef.index; // 인덱스?
                newRoyalTitleDef.inheritanceWorkerOverrideClass = royalTitleDef.inheritanceWorkerOverrideClass;
                newRoyalTitleDef.label = royalTitleDef.label;
                newRoyalTitleDef.labelFemale = royalTitleDef.labelFemale;
                newRoyalTitleDef.lostThought = royalTitleDef.lostThought;
                newRoyalTitleDef.minExpectation = royalTitleDef.minExpectation;
                newRoyalTitleDef.modContentPack = royalTitleDef.modContentPack;
                newRoyalTitleDef.modExtensions = royalTitleDef.modExtensions;
                //n.needFallPerDayAuthority = b.needFallPerDayAuthority;
                if (royalTitleDef.permits != null)
                {
                    newRoyalTitleDef.permits = [];
                    for (var j = 0; j < royalTitleDef.permits.Count; j++)
                    {
                        var newPermit = royalTitleDef.permits[j];

                        switch (newPermit.defName)
                        {
                            case "TradeSettlement":
                                newPermit = tradeSettlementPermit;
                                break;
                            case "TradeOrbital":
                                newPermit = tradeOrbitalPermit;
                                break;
                            case "TradeCaravan":
                                newPermit = tradeCaravanPermit;
                                break;

                            case "CallMilitaryAidSmall":
                                newPermit = callMilitaryAidSmall;
                                break;
                            case "CallMilitaryAidLarge":
                                newPermit = callMilitaryAidLarge;
                                break;
                            case "CallMilitaryAidGrand":
                                newPermit = callMilitaryAidGrand;
                                break;
                        }

                        newRoyalTitleDef.permits.Add(newPermit);
                    }
                }

                newRoyalTitleDef.recruitmentResistanceOffset = royalTitleDef.recruitmentResistanceOffset;
                newRoyalTitleDef.replaceOnRecruited = royalTitleDef.replaceOnRecruited;
                newRoyalTitleDef.requiredApparel =
                    useApparel != ApparelType.off ? royalTitleDef.requiredApparel : null;

                newRoyalTitleDef.requiredMinimumApparelQuality = royalTitleDef.requiredMinimumApparelQuality;
                newRoyalTitleDef.rewards = royalTitleDef.rewards;
                newRoyalTitleDef.seniority = royalTitleDef.seniority;
                newRoyalTitleDef.shortHash = royalTitleDef.shortHash;
                newRoyalTitleDef.suppressIdleAlert = royalTitleDef.suppressIdleAlert;
                newRoyalTitleDef.throneRoomRequirements = royalTitleDef.throneRoomRequirements;

                DefDatabase<RoyalTitleDef>.Add(newRoyalTitleDef);

                newRoyalTitleDefList.Add(newRoyalTitleDef);
                if (i < royalTitleDefList.Count - 2)
                {
                    newRoyalTitleDefListNoStella.Add(newRoyalTitleDef);
                }

                if (i < royalTitleDefList.Count - 1)
                {
                    newRoyalTitleDefListNoEmperor.Add(newRoyalTitleDef);
                }
            }

            if (debugMode)
            {
                Log.Message("E");
            }


            // 귀족 PawnKind 생성


            // 버그 해결 모드
            if (allPawns.Count > 0 && bugFixMode)
            {
                Log.Message($" - {faction.defName} : make pawnkind with bugFix mode");

                var needMakeLeader = allPawns.Count >= 6 || allPawnsLeaders.Count > 0;

                // pawn을 귀족으로 변환

                for (var i = 0; i < allPawnsNoLeaders.Count; i++)
                {
                    var pawnKindDef = allPawns[i];
                    pawnKindDef.titleSelectOne =
                        needMakeLeader ? newRoyalTitleDefListNoStella : newRoyalTitleDefListNoEmperor;

                    pawnKindDef.royalTitleChance = 1f;
                    pawnKindDef.allowRoyalApparelRequirements = false; // 복장 요구 여부
                    if (pawnKindDef.techHediffsTags == null)
                    {
                        pawnKindDef.techHediffsTags = [];
                    }

                    pawnKindDef.techHediffsTags.AddRange(new List<string>
                        { "Advanced", "ImplantEmpireRoyal", "ImplantEmpireCommon" });
                }


                // pawn을 팩션리더 귀족으로 변환

                if (needMakeLeader)
                {
                    var pawnKindDef = allPawnsLeaders.Count > 0
                        ? allPawnsLeaders.RandomElement()
                        : allPawns[allPawns.Count - 1];

                    pawnKindDef.titleSelectOne = [];
                    pawnKindDef.titleRequired =
                        newRoyalTitleDefListNoEmperor[newRoyalTitleDefListNoEmperor.Count - 1];
                    pawnKindDef.royalTitleChance = 1f;
                    pawnKindDef.allowRoyalApparelRequirements = false;
                    if (pawnKindDef.techHediffsTags == null)
                    {
                        pawnKindDef.techHediffsTags = [];
                    }

                    pawnKindDef.techHediffsTags.AddRange(new List<string>
                        { "Advanced", "ImplantEmpireRoyal", "ImplantEmpireCommon" });
                }
            }


            // 일반 모드
            if (allPawns.Count > 0 && !bugFixMode)
            {
                Log.Message($" - {faction.defName} : make pawnkind");

                for (var i = 1; i < newRoyalTitleDefList.Count - 1; i++)
                {
                    var royalTitleDef = newRoyalTitleDefList[i];
                    var randomPawn = allPawnsNoLeaders[allPawnsNoLeaders.Count - 1];
                    var newPawn = new PawnKindDef();
                    var titleBasePawn = royalPawnKindDefList[i - 1];

                    if (i == newRoyalTitleDefList.Count - 2 && allPawnsLeaders.Count > 0)
                    {
                        randomPawn = allPawnsLeaders.RandomElement(); // stellach 계급은 무조건 리더이어야하므로 리더 값을 베이스로 생성
                    }

                    newPawn.defName = $"royal_{royalTitleDef.defName}_{faction.defName}"; //
                    newPawn.label = titleBasePawn.label; // 라벨
                    newPawn.titleRequired = newRoyalTitleDefList[i]; // 정해진 계급

                    newPawn.aiAvoidCover = titleBasePawn.aiAvoidCover;

                    switch (useApparel) // 귀족 폰이 의복을 요구할지 여부
                    {
                        case ApparelType.off:
                            // 사용안함
                            newPawn.allowRoyalApparelRequirements = false; // 복장 요구 여부
                            newPawn.apparelDisallowTags = randomPawn.apparelDisallowTags; // 금지 복장
                            newPawn.apparelTags = randomPawn.apparelTags; // 허용복장
                            newPawn.apparelColor = randomPawn.apparelColor;
                            break;
                        case ApparelType.forced:
                            // 바닐라
                            newPawn.allowRoyalApparelRequirements =
                                titleBasePawn.allowRoyalApparelRequirements; // 복장 요구 여부
                            newPawn.apparelTags = ["IndustrialBasic"];
                            newPawn.apparelDisallowTags = titleBasePawn.apparelDisallowTags; // 금지 복장
                            newPawn.specificApparelRequirements =
                                titleBasePawn.specificApparelRequirements; // 귀족 의상 강제
                            newPawn.apparelColor = titleBasePawn.apparelColor;
                            break;
                        default:
                            // 모드아이템
                            newPawn.allowRoyalApparelRequirements =
                                titleBasePawn.allowRoyalApparelRequirements; // 복장 요구 여부
                            newPawn.apparelTags = randomPawn.apparelTags; // 허용 복장
                            newPawn.specificApparelRequirements =
                                titleBasePawn.specificApparelRequirements; // 귀족 의상 강제
                            newPawn.apparelColor = randomPawn.apparelColor;
                            break;
                    }

                    newPawn.allowRoyalRoomRequirements = titleBasePawn.allowRoyalRoomRequirements;

                    newPawn.alternateGraphicChance = randomPawn.alternateGraphicChance;
                    newPawn.alternateGraphics = randomPawn.alternateGraphics;
                    newPawn.apparelAllowHeadgearChance = titleBasePawn.apparelAllowHeadgearChance; //


                    newPawn.apparelIgnoreSeasons = titleBasePawn.apparelIgnoreSeasons; //
                    newPawn.apparelMoney = titleBasePawn.apparelMoney; // 복장 가격
                    newPawn.apparelRequired = titleBasePawn.apparelRequired; // 복장 강제


                    newPawn.backstoryCategories = randomPawn.backstoryCategories;
                    newPawn.backstoryCryptosleepCommonality = randomPawn.backstoryCryptosleepCommonality;
                    newPawn.backstoryFilters = randomPawn.backstoryFilters;
                    newPawn.backstoryFiltersOverride = randomPawn.backstoryFiltersOverride;
                    newPawn.biocodeWeaponChance = titleBasePawn.biocodeWeaponChance; //
                    newPawn.canArriveManhunter = randomPawn.canArriveManhunter;
                    newPawn.canBeSapper = randomPawn.canBeSapper;
                    newPawn.chemicalAddictionChance = randomPawn.chemicalAddictionChance;
                    newPawn.combatEnhancingDrugsChance = randomPawn.combatEnhancingDrugsChance;
                    newPawn.combatEnhancingDrugsCount = randomPawn.combatEnhancingDrugsCount;
                    newPawn.combatPower = randomPawn.combatPower;
                    newPawn.debugRandomId = randomPawn.debugRandomId;
                    newPawn.defaultFactionType = faction; // 기본 팩션
                    newPawn.defendPointRadius = randomPawn.defendPointRadius;

                    newPawn.description = randomPawn.description;
                    newPawn.descriptionHyperlinks = randomPawn.descriptionHyperlinks;
                    newPawn.destroyGearOnDrop = randomPawn.destroyGearOnDrop;

                    newPawn.disallowedTraits = randomPawn.disallowedTraits == null
                        ? []
                        : randomPawn.disallowedTraits.ListFullCopy();

                    newPawn.disallowedTraits.Add(TraitDefOf.Nudist);
                    newPawn.disallowedTraits.Add(TraitDefOf.Brawler);


                    newPawn.ecoSystemWeight = randomPawn.ecoSystemWeight;
                    newPawn.factionLeader = titleBasePawn.factionLeader; // 팩션리더?
                    newPawn.fileName = randomPawn.fileName; // 파일명
                    newPawn.fixedInventory = randomPawn.fixedInventory;
                    newPawn.fleeHealthThresholdRange = randomPawn.fleeHealthThresholdRange;
                    newPawn.forceNormalGearQuality = randomPawn.forceNormalGearQuality;
                    newPawn.gearHealthRange = randomPawn.gearHealthRange;
                    newPawn.generated = titleBasePawn.generated; // 생성완료?
                    newPawn.acceptArrestChanceFactor = titleBasePawn.acceptArrestChanceFactor;
                    newPawn.ignoreConfigErrors = randomPawn.ignoreConfigErrors;
                    newPawn.index = titleBasePawn.index; // 인덱스?
                    newPawn.inventoryOptions = titleBasePawn.inventoryOptions;
                    newPawn.invFoodDef = titleBasePawn.invFoodDef;
                    newPawn.invNutrition = titleBasePawn.invNutrition;
                    newPawn.isFighter = titleBasePawn.isFighter;
                    newPawn.itemQuality = titleBasePawn.itemQuality;

                    newPawn.labelFemale = randomPawn.labelFemale;
                    newPawn.labelFemalePlural = randomPawn.labelFemalePlural;
                    newPawn.labelMale = randomPawn.labelMale;
                    newPawn.labelMalePlural = randomPawn.labelMalePlural;
                    newPawn.labelPlural = randomPawn.labelPlural;
                    newPawn.lifeStages = randomPawn.lifeStages;
                    newPawn.maxGenerationAge = randomPawn.maxGenerationAge;
                    newPawn.minGenerationAge = randomPawn.minGenerationAge;
                    newPawn.modContentPack = randomPawn.modContentPack; // 모드?
                    newPawn.modExtensions = randomPawn.modExtensions;
                    newPawn.race = randomPawn.race; // 종족?
                    newPawn.royalTitleChance = 1f; // 귀족으로 전환할 확률
                    newPawn.shortHash = randomPawn.shortHash;
                    newPawn.skills = randomPawn.skills;

                    newPawn.techHediffsChance = titleBasePawn.techHediffsChance;

                    newPawn.techHediffsDisallowTags = randomPawn.techHediffsDisallowTags == null
                        ? []
                        : randomPawn.techHediffsDisallowTags.ListFullCopy();

                    newPawn.techHediffsDisallowTags.Add("PainCauser");


                    newPawn.techHediffsMaxAmount = titleBasePawn.techHediffsMaxAmount;
                    newPawn.techHediffsMoney = titleBasePawn.techHediffsMoney;
                    newPawn.techHediffsRequired = titleBasePawn.techHediffsRequired;

                    newPawn.techHediffsTags = randomPawn.techHediffsTags == null
                        ? []
                        : randomPawn.techHediffsTags.ListFullCopy();

                    newPawn.techHediffsTags.AddRange(new List<string>
                        { "Advanced", "ImplantEmpireRoyal", "ImplantEmpireCommon" });


                    newPawn.trader = false; // 상인
                    newPawn.weaponMoney = titleBasePawn.weaponMoney;
                    newPawn.weaponTags = randomPawn.weaponTags;
                    newPawn.wildGroupSize = titleBasePawn.wildGroupSize;

                    DefDatabase<PawnKindDef>.Add(newPawn);
                }
            }


            // Pawn Kinds 생성 : SpaceRefugee_Clothed
            if (allPawnsNoLeaders.Count > 0)
            {
                var spaceRefugeeClothedDef = PawnKindDef.Named("SpaceRefugee_Clothed");
                var randomPawnCopy = CopyPawnDef(allPawnsNoLeaders[0]);
                randomPawnCopy.defName = $"SpaceRefugee_Clothed_{faction.defName}";
                randomPawnCopy.apparelMoney = spaceRefugeeClothedDef.apparelMoney;
                randomPawnCopy.gearHealthRange = spaceRefugeeClothedDef.gearHealthRange;
                if (randomPawnCopy.disallowedTraits == null)
                {
                    randomPawnCopy.disallowedTraits = [];
                }

                randomPawnCopy.disallowedTraits.Add(TraitDefOf.Nudist);
                randomPawnCopy.forceNormalGearQuality = spaceRefugeeClothedDef.forceNormalGearQuality;
                randomPawnCopy.isFighter = spaceRefugeeClothedDef.isFighter;
                randomPawnCopy.apparelAllowHeadgearChance = spaceRefugeeClothedDef.apparelAllowHeadgearChance;
                randomPawnCopy.techHediffsMoney = spaceRefugeeClothedDef.techHediffsMoney;
                randomPawnCopy.techHediffsTags = spaceRefugeeClothedDef.techHediffsTags;
                randomPawnCopy.techHediffsChance = spaceRefugeeClothedDef.techHediffsChance;

                DefDatabase<PawnKindDef>.Add(randomPawnCopy);
            }
            else
            {
                var spaceRefugeeClothedDef = PawnKindDef.Named("SpaceRefugee_Clothed");
                var randomPawnCopy = CopyPawnDef(spaceRefugeeClothedDef);
                randomPawnCopy.defName = $"SpaceRefugee_Clothed_{faction.defName}";

                DefDatabase<PawnKindDef>.Add(randomPawnCopy);
            }

            if (debugMode)
            {
                Log.Message("F");
            }

            // 커스텀 계급 규칙
            var royalImplantRuleList = new List<RoyalImplantRule>();
            if (empireFactionDef.royalImplantRules != null)
            {
                for (var i = 0; i < empireFactionDef.royalImplantRules.Count; i++)
                {
                    // 복제
                    var baseRoyalImplantRule = empireFactionDef.royalImplantRules[i];
                    var newRoyalImplantRule = new RoyalImplantRule
                    {
                        implantHediff = baseRoyalImplantRule.implantHediff,
                        maxLevel = baseRoyalImplantRule.maxLevel,
                        minTitle = newRoyalTitleDefList[i]
                    };

                    royalImplantRuleList.Add(newRoyalImplantRule);
                }
            }

            faction.royalImplantRules = royalImplantRuleList;

            // 계급에 따른 거래제한
            if (useTradePermit)
            {
                _ = new RoyalTitlePermitDef();

                // 기지
                var traderKindDefList = new List<TraderKindDef>();
                foreach (var traderKindDef in faction.baseTraderKinds)
                {
                    var newTraderKindDef = CopyTraderDef(traderKindDef);
                    newTraderKindDef.defName = $"{traderKindDef.defName}_{faction.defName}_base";
                    newTraderKindDef.permitRequiredForTrading = tradeSettlementPermit;
                    newTraderKindDef.faction = faction;
                    DefDatabase<TraderKindDef>.Add(newTraderKindDef);
                    traderKindDefList.Add(newTraderKindDef);
                }

                faction.baseTraderKinds = traderKindDefList;


                // 캐러밴
                traderKindDefList = [];
                foreach (var caravanTraderKindDef in faction.caravanTraderKinds)
                {
                    var newCaravanTraderKindDef = CopyTraderDef(caravanTraderKindDef);
                    newCaravanTraderKindDef.defName =
                        $"{caravanTraderKindDef.defName}_{faction.defName}_caravan";
                    newCaravanTraderKindDef.permitRequiredForTrading = tradeCaravanPermit;
                    newCaravanTraderKindDef.faction = faction;
                    DefDatabase<TraderKindDef>.Add(newCaravanTraderKindDef);
                    traderKindDefList.Add(newCaravanTraderKindDef);
                }

                faction.caravanTraderKinds = traderKindDefList;

                // 궤도상선
                foreach (var traderKindDef in from traders in DefDatabase<TraderKindDef>.AllDefs
                         where
                             traders.orbital && traders.faction != null && traders.faction == faction
                         select traders
                        )
                {
                    traderKindDef.permitRequiredForTrading = tradeOrbitalPermit;
                }
            }

            if (debugMode)
            {
                Log.Message("G");
            }
        }
    }


    public static void PatchRelation()
    {
        for (var factionDef = 0; factionDef < factionDefList.Count; factionDef++)
        {
            var faction = factionDefList[factionDef];


            // 관계
            switch (relationList[factionDef])
            {
                case RelationType.basic:
                    break;
                case RelationType.empire:
                    faction.permanentEnemy = false;
                    faction.mustStartOneEnemy = false;
                    break;
                case RelationType.ally:
                    faction.permanentEnemy = false;
                    faction.mustStartOneEnemy = false;
                    break;
                case RelationType.neutral:
                    faction.permanentEnemy = false;
                    faction.mustStartOneEnemy = false;
                    break;
                case RelationType.enemy:
                    faction.permanentEnemy = false;
                    faction.mustStartOneEnemy = false;
                    break;
                case RelationType.permanentEnemy:
                    faction.permanentEnemy = true;
                    break;
            }


            if (factionToEmpireList[factionDef] == MakeType.no)
            {
                continue;
            }

            // 적대 세력일 경우
            if (!faction.permanentEnemy)
            {
                continue;
            }

            faction.permanentEnemy = false;
            faction.mustStartOneEnemy = false;
        }
    }


    public static PawnKindDef CopyPawnDef(PawnKindDef pawnKindDef)
    {
        var newPawnKindDef = new PawnKindDef
        {
            defName = pawnKindDef.defName,
            label = pawnKindDef.label,
            aiAvoidCover = pawnKindDef.aiAvoidCover,
            allowRoyalApparelRequirements = pawnKindDef.allowRoyalApparelRequirements,
            allowRoyalRoomRequirements = pawnKindDef.allowRoyalRoomRequirements,
            alternateGraphicChance = pawnKindDef.alternateGraphicChance,
            alternateGraphics = pawnKindDef.alternateGraphics,
            apparelAllowHeadgearChance = pawnKindDef.apparelAllowHeadgearChance, //
            apparelColor = pawnKindDef.apparelColor,
            apparelDisallowTags = pawnKindDef.apparelDisallowTags,
            apparelIgnoreSeasons = pawnKindDef.apparelIgnoreSeasons,
            apparelMoney = pawnKindDef.apparelMoney,
            apparelRequired = pawnKindDef.apparelRequired,
            apparelTags = pawnKindDef.apparelTags,
            backstoryCategories = pawnKindDef.backstoryCategories,
            backstoryCryptosleepCommonality = pawnKindDef.backstoryCryptosleepCommonality,
            backstoryFilters = pawnKindDef.backstoryFilters,
            backstoryFiltersOverride = pawnKindDef.backstoryFiltersOverride,
            biocodeWeaponChance = pawnKindDef.biocodeWeaponChance, //
            canArriveManhunter = pawnKindDef.canArriveManhunter,
            canBeSapper = pawnKindDef.canBeSapper,
            chemicalAddictionChance = pawnKindDef.chemicalAddictionChance,
            combatEnhancingDrugsChance = pawnKindDef.combatEnhancingDrugsChance,
            combatEnhancingDrugsCount = pawnKindDef.combatEnhancingDrugsCount,
            combatPower = pawnKindDef.combatPower,
            debugRandomId = pawnKindDef.debugRandomId,
            defaultFactionType = pawnKindDef.defaultFactionType,
            defendPointRadius = pawnKindDef.defendPointRadius,
            description = pawnKindDef.description,
            descriptionHyperlinks = pawnKindDef.descriptionHyperlinks,
            destroyGearOnDrop = pawnKindDef.destroyGearOnDrop,
            disallowedTraits = pawnKindDef.disallowedTraits,
            ecoSystemWeight = pawnKindDef.ecoSystemWeight,
            factionLeader = pawnKindDef.factionLeader, // 팩션리더?
            fileName = pawnKindDef.fileName,
            fixedInventory = pawnKindDef.fixedInventory,
            fleeHealthThresholdRange = pawnKindDef.fleeHealthThresholdRange,
            forceNormalGearQuality = pawnKindDef.forceNormalGearQuality,
            gearHealthRange = pawnKindDef.gearHealthRange,
            generated = pawnKindDef.generated, // 생성완료?
            acceptArrestChanceFactor = pawnKindDef.acceptArrestChanceFactor,
            ignoreConfigErrors = pawnKindDef.ignoreConfigErrors,
            index = pawnKindDef.index, // 인덱스?
            inventoryOptions = pawnKindDef.inventoryOptions,
            invFoodDef = pawnKindDef.invFoodDef,
            invNutrition = pawnKindDef.invNutrition,
            isFighter = pawnKindDef.isFighter,
            itemQuality = pawnKindDef.itemQuality,
            labelFemale = pawnKindDef.labelFemale,
            labelFemalePlural = pawnKindDef.labelFemalePlural,
            labelMale = pawnKindDef.labelMale,
            labelMalePlural = pawnKindDef.labelMalePlural,
            labelPlural = pawnKindDef.labelPlural,
            lifeStages = pawnKindDef.lifeStages,
            maxGenerationAge = pawnKindDef.maxGenerationAge,
            minGenerationAge = pawnKindDef.minGenerationAge,
            modContentPack = pawnKindDef.modContentPack, // 모드?
            modExtensions = pawnKindDef.modExtensions,
            race = pawnKindDef.race, // 종족?
            royalTitleChance = 1f,
            shortHash = pawnKindDef.shortHash,
            skills = pawnKindDef.skills,
            specificApparelRequirements = pawnKindDef.specificApparelRequirements, // 귀족 의상
            techHediffsChance = pawnKindDef.techHediffsChance,
            techHediffsDisallowTags = pawnKindDef.techHediffsDisallowTags,
            techHediffsMaxAmount = pawnKindDef.techHediffsMaxAmount,
            techHediffsMoney = pawnKindDef.techHediffsMoney,
            techHediffsRequired = pawnKindDef.techHediffsRequired,
            techHediffsTags = pawnKindDef.techHediffsTags,
            titleRequired = pawnKindDef.titleRequired, // 정해진 계급
            trader = pawnKindDef.trader,
            weaponMoney = pawnKindDef.weaponMoney,
            weaponTags = pawnKindDef.weaponTags,
            wildGroupSize = pawnKindDef.wildGroupSize
        };

        return newPawnKindDef;
    }

    public static TraderKindDef CopyTraderDef(TraderKindDef traderKindDef)
    {
        var newTraderKindDef = new TraderKindDef
        {
            defName = traderKindDef.defName,
            permitRequiredForTrading = traderKindDef.permitRequiredForTrading,
            faction = traderKindDef.faction,
            category = traderKindDef.category,
            commonality = traderKindDef.commonality,
            commonalityMultFromPopulationIntent = traderKindDef.commonalityMultFromPopulationIntent,
            description = traderKindDef.description,
            descriptionHyperlinks = traderKindDef.descriptionHyperlinks,
            hideThingsNotWillingToTrade = traderKindDef.hideThingsNotWillingToTrade,
            ignoreConfigErrors = traderKindDef.ignoreConfigErrors,
            label = traderKindDef.label,
            modContentPack = traderKindDef.modContentPack,
            modExtensions = traderKindDef.modExtensions,
            orbital = traderKindDef.orbital,
            requestable = traderKindDef.requestable,
            shortHash = traderKindDef.shortHash,
            stockGenerators = traderKindDef.stockGenerators,
            tradeCurrency = traderKindDef.tradeCurrency
        };

        return newTraderKindDef;
    }


    public static bool IsViolatingRulesOf(Def implantOrWeapon, Pawn pawn, Faction faction, int implantLevel = 0)
    {
        if (faction.def.royalImplantRules == null || faction.def.royalImplantRules.Count == 0)
        {
            return true;
        }

        var minTitleToUse =
            ThingRequiringRoyalPermissionUtility.GetMinTitleToUse(implantOrWeapon, faction, implantLevel);
        var currentTitle = pawn.royalty.GetCurrentTitle(faction);
        if (currentTitle == null)
        {
            return true;
        }

        var num = faction.def.RoyalTitlesAwardableInSeniorityOrderForReading.IndexOf(currentTitle);
        if (num < 0)
        {
            return true;
        }

        var num2 = faction.def.RoyalTitlesAwardableInSeniorityOrderForReading.IndexOf(minTitleToUse);
        return num < num2;
    }
}