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
    /*
    [HarmonyPatch(typeof(QuestGen_Pawns), "GeneratePawn", new Type[]
    {
        typeof(Quest),
        typeof(PawnKindDef),
        typeof(Faction),
        typeof(bool),
        typeof(IEnumerable<TraitDef>),
        typeof(float),
        typeof(bool),
        typeof(Pawn),
        typeof(float),
        typeof(float),
        typeof(bool),
        typeof(bool)
    })]
    */
    // QuestNode_Root_Hospitality_Refugee
    public static class QuestGen_Pawns_GeneratePawn_Patch
    {
    //    [HarmonyPostfix]
        public static void GeneratePawn_Prefix(Quest quest, ref PawnKindDef kindDef, ref Faction faction)
        {
            if (kindDef == PawnKindDefOf.Refugee )
            {
                if (Faction.OfPlayer.def == OGOrkFactionDefOf.OG_Ork_PlayerTribe)
                {
                    kindDef = OGOrkPawnKindDefOf.Refugee_FeralOrk;
                }
                else
                if (Faction.OfPlayer.def == OGOrkFactionDefOf.OG_Grot_PlayerTribe)
                {

                    kindDef = OGOrkPawnKindDefOf.Refugee_FeralGrot;
                }
            }
            /*
            if (faction?.def.defName == "OutlanderRefugee")
            {
                if (Faction.OfPlayer.def == OGOrkFactionDefOf.OG_Ork_PlayerTribe)
                {
                    faction = OGOrkFactionDefOf.OG_Ork_FeralRefugee_Faction;
                }
                else
                if (Faction.OfPlayer.def == OGOrkFactionDefOf.OG_Grot_PlayerTribe)
                {

                    faction = OGOrkFactionDefOf.OG_Ork_FeralRefugee_Faction;
                }
            }
            */
        }
    }

}
