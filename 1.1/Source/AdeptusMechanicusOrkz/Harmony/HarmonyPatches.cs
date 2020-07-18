using System;
using System.Collections.Generic;
using System.Linq;
using FeralOrkz.ExtensionMethods;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace FeralOrkz
{
    [StaticConstructorOnStartup]
    public static class HarmonyMain
    {
        private static readonly Type patchType = typeof(HarmonyMain);
        static HarmonyMain()
        {
            var harmony = new Harmony("RimWorld.Ogliss.FeralOrkz");

            harmony.Patch(
                original: AccessTools.Method(type: typeof(FoodUtility), name: "AddFoodPoisoningHediff"),
                prefix: new HarmonyMethod(patchType, nameof(Prefix_AddFoodPoisoningHediff_Orkoid)),
                postfix: null);
            harmony.Patch(
                original: AccessTools.Method(type: typeof(Verb_MeleeAttackDamage), name: "DamageInfosToApply"),
                prefix: null,
                postfix: new HarmonyMethod(patchType, nameof(Postfix_ForceWeaponAttack)));
            harmony.Patch(
                original: AccessTools.Method(type: typeof(PawnBioAndNameGenerator), name: "GiveShuffledBioTo"),
                prefix: new HarmonyMethod(patchType, nameof(Prefix_GiveShuffledBioTo_Orkoid)),
                postfix: null);
        }

        public static bool Prefix_AddFoodPoisoningHediff_Orkoid(Pawn pawn, Thing ingestible, FoodPoisonCause cause)
        {
            if (pawn.kindDef.race == OGOrkThingDefOf.OG_Alien_Ork || pawn.kindDef.race == OGOrkThingDefOf.OG_Alien_Grot || pawn.kindDef.race == OGOrkThingDefOf.OG_Snotling || pawn.kindDef.race == OGOrkThingDefOf.OG_Squig || pawn.kindDef.race == OGOrkThingDefOf.OG_Squig_Ork)
            {
                if (ingestible.def.ingestible.foodType == FoodTypeFlags.Meat)
                {
                    return false;
                }
                if (cause == FoodPoisonCause.DangerousFoodType)
                {
                    return false;
                }
            }
            return true;
        }

        public static IEnumerable<DamageInfo> Postfix_ForceWeaponAttack(IEnumerable<DamageInfo> __result, Verb_MeleeAttackDamage __instance, LocalTargetInfo target)
        {
            foreach (DamageInfo info in __result)
            {
                bool returnoriginal = true;
                if (info.Def.forceWeapon())
                {
                    returnoriginal = false;
                    DamageInfo dinfo = GetForceDamage(info, __instance, target);
                    yield return dinfo;
                }
                if (returnoriginal)
                {
                    yield return info;
                }
            }
            yield break;
        }

        public static DamageInfo GetForceDamage(DamageInfo cloneSource, Verb_MeleeAttackDamage __instance, LocalTargetInfo target)
        {
            Pawn Caster = __instance.CasterPawn as Pawn;
            if (Caster != null)
            {
                if (Caster.isPsyker(out int Level, out float Mult))
                {
                    if (__instance.EquipmentSource != null)
                    {
                        CompForceWeaponActivatableEffect WeaponRules = __instance.EquipmentSource.TryGetComp<CompForceWeaponActivatableEffect>();
                        if (WeaponRules != null)
                        {
                            if (WeaponRules.ForceWeapon)
                            {
                                bool casterPsychiclySensitive = Caster.RaceProps.Humanlike ? Caster.story.traits.HasTrait(TraitDefOf.PsychicSensitivity) || Caster.story.traits.HasTrait(DefDatabase<TraitDef>.GetNamedSilentFail("Psyker")) : false;
                                bool Activate = false;
                                if ((casterPsychiclySensitive || !WeaponRules.ForceEffectRequiresPsyker) && target.Thing.def.category == ThingCategory.Pawn && target.Thing is Pawn Victim)
                                {
                                    int casterPsychiclySensitiveDegree = casterPsychiclySensitive ? Caster.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity) : 0;
                                    if ((casterPsychiclySensitiveDegree >= 1 || !WeaponRules.ForceEffectRequiresPsyker))
                                    {
                                        float? casterPsychicSensitivity = Caster.GetStatValue(StatDefOf.PsychicSensitivity, true) * 100f;
                                        bool targetPsychiclySensitive = Victim.RaceProps.Humanlike ? Victim.story.traits.HasTrait(TraitDefOf.PsychicSensitivity) : false;
                                        float? targetPsychicSensitivity = Victim.GetStatValue(StatDefOf.PsychicSensitivity, true) * 100f;
                                        if (targetPsychiclySensitive == true)
                                        {
                                            int targetPsychiclySensitiveDegree = Victim.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity);
                                            if (targetPsychiclySensitiveDegree == -1) { targetPsychicSensitivity = Victim.def.statBases.GetStatValueFromList(StatDefOf.PsychicSensitivity, 1.5f) * 100f; }
                                            else if (targetPsychiclySensitiveDegree == -2) { targetPsychicSensitivity = Victim.def.statBases.GetStatValueFromList(StatDefOf.PsychicSensitivity, 2f) * 100f; }
                                        }
                                        else { /*int targetPsychiclySensitiveDegree = 0;*/ }
                                        {
                                            float CasterMood = Caster.needs.mood.CurLevelPercentage;
                                            float VictimMood = Victim?.needs?.mood != null ? Victim.needs.mood.CurLevelPercentage : 1;
                                            Rand.PushState();
                                            float? casterRoll = Rand.Range(0, (int)casterPsychicSensitivity) * CasterMood;
                                            float? targetRoll = Rand.Range(0, (int)targetPsychicSensitivity) * VictimMood;
                                            Rand.PopState();
                                            casterRoll = (casterRoll - (targetPsychicSensitivity / 2));
                                            Activate = (casterRoll > targetRoll);
                                            //    log.message(string.Format("Caster:{0}, Victim:{1}", casterRoll, targetRoll));
                                            if (Activate)
                                            {
                                                DamageDef damDef = WeaponRules.ForceWeaponEffect;
                                                float damAmount = __instance.verbProps.AdjustedMeleeDamageAmount(__instance, __instance.CasterPawn);
                                                float armorPenetration = __instance.verbProps.AdjustedArmorPenetration(__instance, __instance.CasterPawn);
                                                BodyPartRecord bodyPart = Rand.Chance(0.05f) && Victim.RaceProps.body.AllParts.Any(x => x.def.defName.Contains("Brain")) ? Victim.RaceProps.body.AllParts.Find(x => x.def.defName.Contains("Brain")) : null;
                                                BodyPartGroupDef bodyPartGroupDef = null;
                                                HediffDef hediffDef = WeaponRules.ForceWeaponHediff;
                                                damAmount = Rand.Range(damAmount * 0.1f, damAmount * 0.5f);
                                                ThingDef source = __instance.EquipmentSource.def;
                                                Thing caster = __instance.caster;
                                                Vector3 direction = (target.Thing.Position - __instance.CasterPawn.Position).ToVector3();
                                                float num = damAmount;
                                                DamageInfo mainDinfo = new DamageInfo(damDef, num, 2, -1f, caster, bodyPart, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
                                                mainDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                                                mainDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
                                                mainDinfo.SetWeaponHediff(hediffDef);
                                                mainDinfo.SetAngle(direction);
                                                Victim.TakeDamage(mainDinfo);
                                                Map map = Caster.Map;
                                                IntVec3 position = target.Cell;
                                                Map map2 = map;
                                                float explosionRadius = 0f;
                                                Thing launcher = __instance.EquipmentSource;
                                                SoundDef soundExplode = WeaponRules.ForceWeaponTriggerSound;
                                                Thing thing = target.Thing;
                                                GenExplosion.DoExplosion(position, map2, explosionRadius, damDef, launcher, (int)damAmount, armorPenetration, soundExplode, source, null, thing, null, 0f, 0, false, null, 0, 0, 0, false);
                                                float KillChance = WeaponRules.ForceWeaponKillChance;
                                                if (KillChance != 0)
                                                {
                                                    float KillRoll = Rand.Range(0, 100);
                                                    if (Rand.Chance(WeaponRules.ForceWeaponKillChance))
                                                    {
                                                        string msg = string.Format("{0} was slain by a force strike", target.Thing.LabelCap);
                                                        target.Thing.Kill(mainDinfo);
                                                        if (target.Thing.Faction == Faction.OfPlayer) { Messages.Message(msg, MessageTypeDefOf.PawnDeath); }
                                                    }
                                                }
                                                return mainDinfo;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return cloneSource;
        }

        public static bool Prefix_GiveShuffledBioTo_Orkoid(Pawn pawn, FactionDef factionType, string requiredLastName, ref List<BackstoryCategoryFilter> backstoryCategories)
        {
            if (pawn == null)
            {
                return true;
            }
            if (!pawn.RaceProps.Humanlike)
            {
                return true;
            }
            if (pawn.ageTracker == null || pawn.health == null || pawn.story == null || pawn.RaceProps.FleshType != OGOrkThingDefOf.OG_Flesh_Orkoid)
            {
                return true;
            }
            bool ext = pawn.kindDef.HasModExtension<BackstoryExtension>();
            if (ext)
            {
                BackstoryCategoryFilter backstoryCategoryFilter = backstoryCategories.RandomElementByWeight((BackstoryCategoryFilter c) => c.commonality);
                backstoryCategories.Clear();
                backstoryCategories.Add(backstoryCategoryFilter);
            //    Log.Message(pawn.LabelShortCap + " BackstoryExtension using "+ backstoryCategoryFilter.categories.ToCommaList());
            }
            else
            {
            //    Log.Message(pawn.LabelShortCap + " No BackstoryExtension");
            }
            if (pawn.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("TM_") && (x.def.defName.Contains("Undead") || x.def.defName.Contains("Lich"))))
            {
                return true;
            }
            if ((pawn.ageTracker.AgeBiologicalYears < 20 && (pawn.def.defName.StartsWith("OG_") || pawn.kindDef.defName.StartsWith("OG_"))) || ext)
            {
                //    Log.Message(string.Format("AdMech mod pawn: {0} {1} {2}",pawn.NameShortColored, pawn.kindDef, pawn.def.modContentPack.PackageIdPlayerFacing));
                bool act = pawn.RaceProps.lifeStageAges.Any(x => x.def.reproductive);
                if (act)
                {
                    if (pawn.ageTracker.AgeBiologicalYears > pawn.RaceProps.lifeStageAges.First(x => x.def.reproductive).minAge)
                    {
                        FillBackstorySlotShuffled(pawn, BackstorySlot.Childhood, ref pawn.story.childhood, pawn.story.adulthood, backstoryCategories, factionType);
                        if (pawn.ageTracker.AgeBiologicalYearsFloat >= pawn.RaceProps.lifeStageAges.First(x => x.def.reproductive).minAge)
                        {
                            FillBackstorySlotShuffled(pawn, BackstorySlot.Adulthood, ref pawn.story.adulthood, pawn.story.childhood, backstoryCategories, factionType);
                        }
                        pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, requiredLastName);
                        return false;
                    }
                }
            }
            return true;
        }
        public static bool FillBackstorySlotShuffled(Pawn pawn, BackstorySlot slot, ref Backstory backstory, Backstory backstoryOtherSlot, List<BackstoryCategoryFilter> backstoryCategories, FactionDef factionType)
        {
            if (pawn.def.defName.StartsWith("OG_"))
            {
            //    Log.Message(pawn.NameShortColored + " is " +pawn.def + " in " + pawn.Faction);
                BackstoryCategoryFilter backstoryCategoryFilter = backstoryCategories.RandomElementByWeight((BackstoryCategoryFilter c) => c.commonality);
                if (backstoryCategoryFilter == null)
                {
                //    Log.Message(pawn.def + " in " + pawn.Faction + " backstoryCategoryFilter == null");
                    backstoryCategoryFilter = FallbackCategoryGroup;
                //    Log.Message(pawn.def + " in " + pawn.Faction + " backstoryCategoryFilter == " + backstoryCategoryFilter);
                }
                List<string> lista = new List<string>();
                foreach (BackstoryCategoryFilter filter in backstoryCategories)
                {
                    foreach (string str in filter.categories)
                    {
                        if (!lista.Contains(str))
                        {
                            lista.Add(str);
                        }
                    }
                }
                if (!(from bs in BackstoryDatabase.ShuffleableBackstoryList(slot, backstoryCategoryFilter).TakeRandom(20)
                      where slot != BackstorySlot.Adulthood || !bs.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.childhood.workDisables)
                      select bs).TryRandomElementByWeight(new Func<Backstory, float>(BackstorySelectionWeight), out backstory))
                {
                //    Log.Message(string.Format("backstoryCategories: {0}, used backstoryCategoryFilter: {1}", lista.ToCommaList(), backstoryCategoryFilter.categories.ToCommaList()));
                    Log.Error(string.Concat(new object[]
                    {
                    "No shuffled ",
                    slot,
                    " found for ",
                    pawn.ToStringSafe<Pawn>(),
                    " of ",
                    factionType.ToStringSafe<FactionDef>(),
                    ". Choosing random."
                    }), false);
                    backstory = (from kvp in BackstoryDatabase.allBackstories
                                 where kvp.Value.slot == slot
                                 select kvp).RandomElement<KeyValuePair<string, Backstory>>().Value;
                }
                return false;
            }
            return true;
        }

        // Token: 0x060040BF RID: 16575 RVA: 0x00159374 File Offset: 0x00157574
        public static List<BackstoryCategoryFilter> GetBackstoryCategoryFiltersFor(Pawn pawn, FactionDef faction)
        {
            if (!pawn.kindDef.backstoryFiltersOverride.NullOrEmpty<BackstoryCategoryFilter>())
            {
                return pawn.kindDef.backstoryFiltersOverride;
            }
            List<BackstoryCategoryFilter> list = new List<BackstoryCategoryFilter>();
            if (pawn.kindDef.backstoryFilters != null)
            {
                list.AddRange(pawn.kindDef.backstoryFilters);
            }
            if (faction != null && !faction.backstoryFilters.NullOrEmpty<BackstoryCategoryFilter>())
            {
                for (int i = 0; i < faction.backstoryFilters.Count; i++)
                {
                    BackstoryCategoryFilter item = faction.backstoryFilters[i];
                    if (!list.Contains(item))
                    {
                        list.Add(item);
                    }
                }
            }
            if (!list.NullOrEmpty<BackstoryCategoryFilter>())
            {
                return list;
            }
            Log.ErrorOnce(string.Concat(new object[]
            {
                "PawnKind ",
                pawn.kindDef,
                " generating with factionDef ",
                faction,
                ": no backstoryCategories in either."
            }), 1871521, false);
            return new List<BackstoryCategoryFilter>
            {
                FallbackCategoryGroup
            };
        }

        public static readonly BackstoryCategoryFilter FallbackCategoryGroup = new BackstoryCategoryFilter
        {
            categories = new List<string>
            {
                "Civil"
            },
            commonality = 1f
        };

        // Token: 0x06001503 RID: 5379 RVA: 0x000A3B95 File Offset: 0x000A1F95
        public static float BackstorySelectionWeight(Backstory bs)
        {
            return SelectionWeightFactorFromWorkTagsDisabled(bs.workDisables);
        }

        // Token: 0x06001504 RID: 5380 RVA: 0x000A3BA2 File Offset: 0x000A1FA2
        public static float BioSelectionWeight(PawnBio bio)
        {
            return SelectionWeightFactorFromWorkTagsDisabled(bio.adulthood.workDisables | bio.childhood.workDisables);
        }

        // Token: 0x06001505 RID: 5381 RVA: 0x000A3BC0 File Offset: 0x000A1FC0
        public static float SelectionWeightFactorFromWorkTagsDisabled(WorkTags wt)
        {
            float num = 1f;
            if ((wt & WorkTags.ManualDumb) != WorkTags.None)
            {
                num *= 0.4f;
            }
            if ((wt & WorkTags.ManualSkilled) != WorkTags.None)
            {
                num *= 1f;
            }
            if ((wt & WorkTags.Violent) != WorkTags.None)
            {
                num *= 0.5f;
            }
            if ((wt & WorkTags.Caring) != WorkTags.None)
            {
                num *= 0.9f;
            }
            if ((wt & WorkTags.Social) != WorkTags.None)
            {
                num *= 0.5f;
            }
            if ((wt & WorkTags.Intellectual) != WorkTags.None)
            {
                num *= 0.35f;
            }
            if ((wt & WorkTags.Firefighting) != WorkTags.None)
            {
                num *= 0.7f;
            }
            return num;
        }
        private static List<Backstory> tmpBackstories = new List<Backstory>();
    }
}