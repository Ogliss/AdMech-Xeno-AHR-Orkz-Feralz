using System;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        private static readonly Type patchType = typeof(HarmonyPatches);
        static HarmonyPatches()
        {
            var harmony = HarmonyInstance.Create("rimworld.ogliss.adeptusmechanicus.orkz");

            harmony.Patch(
                original: AccessTools.Method(type: typeof(FoodUtility), name: "AddFoodPoisoningHediff"),
                prefix: new HarmonyMethod(type: patchType, name: nameof(Pre_AddFoodPoisoningHediff_Orkoid)),
                postfix: null);
        }

        public static bool Pre_AddFoodPoisoningHediff_Orkoid(Pawn pawn, Thing ingestible, FoodPoisonCause cause)
        {
        //    Log.Message(string.Format("checkin if {0} can get food poisioning from {1} because {2}", pawn.Name, ingestible ,cause));
            if (pawn.kindDef.race == OGOrkThingDefOf.Alien_Ork || pawn.kindDef.race == OGOrkThingDefOf.Alien_Grot || pawn.kindDef.race == OGOrkThingDefOf.Cyborg_Ork || pawn.kindDef.race == OGOrkThingDefOf.Snotling || pawn.kindDef.race == OGOrkThingDefOf.Squig || pawn.kindDef.race == OGOrkThingDefOf.AttackSquig)
            {
                if (ingestible.def.ingestible.foodType == FoodTypeFlags.Meat)
                {
        //            Log.Message(string.Format("stopped {0} getting food poisioning from {1} because {2}", pawn.Name, ingestible, ingestible.def.ingestible.foodType));
                    return false;
                }
                if (cause == FoodPoisonCause.DangerousFoodType)
                {
        //            Log.Message(string.Format("stopped {0} getting food poisioning from {1} because {2}", pawn.Name, ingestible, cause));
                    return false;
                }
            }
            return true;
        }
        /*
        
        public static bool Pre_AddFoodPoisoningHediff_Orkoid(Pawn pawn, Thing ingestible, FoodPoisonCause cause)
        {
        //    Log.Message(string.Format("checkin if {0} can get food poisioning from {1} because {2}", pawn.Name, ingestible ,cause));
            if (pawn.kindDef.race == OGOrkThingDefOf.Alien_Ork || pawn.kindDef.race == OGOrkThingDefOf.Alien_Grot || pawn.kindDef.race == OGOrkThingDefOf.Cyborg_Ork || pawn.kindDef.race == OGOrkThingDefOf.Snotling || pawn.kindDef.race == OGOrkThingDefOf.Squig || pawn.kindDef.race == OGOrkThingDefOf.AttackSquig)
            {
                if (ingestible.def.ingestible.foodType == FoodTypeFlags.Meat)
                {
        //            Log.Message(string.Format("stopped {0} getting food poisioning from {1} because {2}", pawn.Name, ingestible, ingestible.def.ingestible.foodType));
                    return false;
                }
                if (cause == FoodPoisonCause.DangerousFoodType)
                {
        //            Log.Message(string.Format("stopped {0} getting food poisioning from {1} because {2}", pawn.Name, ingestible, cause));
                    return false;
                }
            }
            return true;
        }  

        */
    }
}