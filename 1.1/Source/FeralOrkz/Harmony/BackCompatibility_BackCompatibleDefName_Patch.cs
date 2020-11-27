using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using System.Text.RegularExpressions;

namespace FeralOrkz
{

//    [HarmonyPatch(typeof(BackCompatibility), "BackCompatibleDefName")]
    public static class BackCompatibility_BackCompatibleDefName_Patch
    {
    //    [HarmonyPostfix]
        public static void BackCompatibleDefName_Postfix(Type defType, string defName, bool forDefInjections, ref string __result)
        {
            if (GenDefDatabase.GetDefSilentFail(defType, defName, false) == null)
            {
                string newName = string.Empty;
                //    Log.Message(string.Format("Checking for replacement for {0} Type: {1}", defName, defType));
                if (defType == typeof(ThingDef))
                {
                    /*
                    if (defName.Contains("ChaosDeamon_"))
                    {
                        if (defName.Contains("Corpse_"))
                        {
                            newName = Regex.Replace(defName, "Corpse_ChaosDeamon_", "Corpse_OG_Chaos_Deamon_");
                        }
                        else
                            newName = Regex.Replace(defName, "ChaosDeamon_", "OG_Chaos_Deamon_");
                    }
                    */
                    
                    if (defName.Contains("Apparel_Ork"))
                    {
                        newName = Regex.Replace(defName, "Apparel_Ork", "OGO_Apparel_");
                    }
                    
                }
                if (defType == typeof(FactionDef))
                {
                    if (defName == "FeralOrkFaction")
                    {
                        newName = "OG_Ork_Feral_Faction";
                    }
                    if (defName.Contains("Ork") && defName.Contains("Player"))
                    {
                        newName = "OG_Ork_PlayerTribe";
                    }

                }
                if (defType == typeof(PawnKindDef))
                {
                    List<PawnKindDef> list;
                    if (defName.Contains("Ork"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Ork")).ToList();
                        if (defName.Contains("Choppa"))
                        {
                            list = list.Where(x => x.defName.Contains("Choppa")).ToList();
                        }
                        else
                        if (defName.Contains("Shoota"))
                        {
                            list = list.Where(x => x.defName.Contains("Shoota")).ToList();
                        }
                        else
                        if (defName.Contains("Slugga"))
                        {
                            list = list.Where(x => x.defName.Contains("Slugga")).ToList();
                        }

                        if (defName.Contains("Mek"))
                        {
                            list = list.Where(x => x.defName.Contains("Mek")).ToList();
                        }
                        if (defName.Contains("Warboss"))
                        {
                            list = list.Where(x => x.defName.Contains("Warboss")).ToList();
                        }
                        else
                        if (defName.Contains("Nob"))
                        {
                            list = list.Where(x => x.defName.Contains("Nob")).ToList();
                        }
                        else
                        {
                            list = list.Where(x => !x.defName.Contains("Nob") && !x.defName.Contains("Warboss")).ToList();
                        }
                        newName = list.RandomElement().defName;

                    }
                    if (defName.Contains("Grot"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Grot")).ToList();
                        if (defName.Contains("Colonist"))
                        {
                            list = list.Where(x => x.defName.Contains("Colonist")).ToList();
                        }
                        newName = list.RandomElement().defName;
                    }
                    if (defName.Contains("Snotling"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Snotling")).ToList();

                        newName = list.RandomElement().defName;
                    }
                    if (defName.Contains("Squig"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Squig")).ToList();

                        newName = list.RandomElement().defName;
                    }

                }
                if (defType == typeof(ResearchProjectDef))
                {
                    // Ork Reseach renames
                    if (defName == "OrkTekBase")
                    {
                        newName = "OG_Ork_Tech_Base_T1";
                    }
                    if (defName == "OrkishBrutality")
                    {
                        newName = "OG_Ork_Tech_Weapons_Melee_T1";
                    }
                    if (defName == "OrkishExtremeBrutality")
                    {
                        newName = "OG_Ork_Tech_Weapons_Melee_T2";
                    }
                    if (defName == "OrkishPowerField")
                    {
                        newName = "OG_Ork_Tech_Weapons_Melee_T3";
                    }
                    if (defName == "OrkishCunning")
                    {
                        newName = "OG_Ork_Tech_Weapons_Ranged_T1";
                    }
                    if (defName == "OrkishIntenseCunning")
                    {
                        newName = "OG_Ork_Tech_Weapons_Ranged_T2";
                    }
                    if (defName == "OrkishMekTek")
                    {
                        newName = "OG_Ork_Tech_Base_T2";
                    }
                    if (defName == "OrkishBigMekBrainz")
                    {
                        newName = "OG_Ork_Tech_Base_T3";
                    }
                    if (defName == "OrkishArmour")
                    {
                        newName = "OG_Ork_Tech_Apparel_Armour_T1";
                    }
                    if (defName == "OrkishEavyArmour")
                    {
                        newName = "OG_Ork_Tech_Apparel_Armour_T2";
                    }
                    if (defName == "OrkishMegaArmour")
                    {
                        newName = "OG_Ork_Tech_Apparel_Armour_T3";
                    }
                }
                if (defType == typeof(HediffDef))
                {
                    if (defName == "HyperactiveNymuneOrgan")
                    {
                        newName = "OG_Kroot_Mutation_HyperactiveNymuneOrgan";
                    }
                }
                if (defType == typeof(BodyDef))
                {
                    /*
                    if (defName.Contains("Ork") || defName.Contains("Ork"))
                    {
                        newName = defName + "_Body";
                    }
                    */
                }
                if (defType == typeof(ScenarioDef))
                {
                    if (defName == "OG_Ork_Tek_Scenario_Test")
                    {
                        newName = "OGAM_Scenario__Ork_Crashlanded";
                    }
                    if (defName == "OG_Ork_Feral_Tribe")
                    {
                        newName = "OGAM_Scenario_Ork_LostTribe";
                    }

                }
                if (!newName.NullOrEmpty())
                {
                    __result = newName;
                }
                if (defName == __result)
                {
                    //    Log.Warning(string.Format("AMA No replacement found for: {0} T:{1}", defName, defType));
                }
                else
                {
                    //    Log.Message(string.Format("Replacement found: {0} T:{1}", __result, defType));
                }
            }
        }
    }

}
