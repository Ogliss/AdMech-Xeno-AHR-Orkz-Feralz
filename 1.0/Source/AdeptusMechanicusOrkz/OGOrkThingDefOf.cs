using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000945 RID: 2373
	[DefOf]
	public static class OGOrkThingDefOf
	{
		// Token: 0x06003770 RID: 14192 RVA: 0x001A8272 File Offset: 0x001A6672
		static OGOrkThingDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(OGOrkThingDefOf));
		}

        // Token: 0x04001EE3 RID: 7907
        public static ThingDef Plant_OrkoidCocoon;

        public static ThingDef Plant_OrkFungus;

        public static ThingDef OrkFermentingBarrel;

        public static ThingDef OrkWaart;

        public static ThingDef OrkGrog;

        public static AlienRace.BackstoryDef Ork_Base_Child;

        public static AlienRace.BackstoryDef Grot_Base_Child;

        public static AlienRace.ThingDef_AlienRace Alien_Ork;
        public static AlienRace.ThingDef_AlienRace Cyborg_Ork;

        public static AlienRace.ThingDef_AlienRace Alien_Grot;

        public static ThingDef Snotling;
        public static ThingDef Squig;
        public static ThingDef AttackSquig;
    }
}
