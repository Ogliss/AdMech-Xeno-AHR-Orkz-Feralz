using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;
using Verse.AI;
using Verse.AI.Group;
using Harmony;
using RimWorld.Planet;
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine;


namespace AdeptusMechanicus
{

    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("Ogliss.RimWorld.XenoBiologis.Orkz");
            HarmonyInstance.DEBUG = true;

            harmony.Patch(AccessTools.Method(typeof(WildManUtility), nameof(WildManUtility.IsWildMan)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(WildManUtilityFix)), null, null);
        }

        #region WildManUtility
        // From
        /* Target Method WildManUtility.IsWildMan
        public static bool IsWildMan(this Pawn p)
        {
            return p.kindDef == PawnKindDefOf.WildMan;
        }
        */
        // to
        /*
        if (p.kindDef == OGOrkPawnKindDefOf.WildGrot)
        {
            return p.kindDef == OGOrkPawnKindDefOf.WildGrot;
        }
        else if(p.kindDef == OGOrkPawnKindDefOf.WildOrk)
        {
            return p.kindDef == OGOrkPawnKindDefOf.WildOrk;
        }
        else return p.kindDef == PawnKindDefOf.WildMan;
         */

        private static bool WildManUtilityFix(this Pawn p, ref bool __result)
        {
            if (p.kindDef == OGOrkPawnKindDefOf.WildGrot)
            {
                return p.kindDef == OGOrkPawnKindDefOf.WildGrot;
            }
            if (p.kindDef == OGOrkPawnKindDefOf.WildOrk)
            {
                return p.kindDef == OGOrkPawnKindDefOf.WildOrk;
            }
            else return p.kindDef == PawnKindDefOf.WildMan;
        }
        #endregion
    }   
}
