using System;
using Verse;

namespace RimWorld
{
    // Token: 0x02000953 RID: 2387
    [DefOf]
    public static class OGOrkPawnKindDefOf
    {
        // Token: 0x0600377C RID: 14204 RVA: 0x001A83CC File Offset: 0x001A67CC
        static OGOrkPawnKindDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGOrkPawnKindDefOf));
        }

        public static PawnKindDef OG_Squig;
        public static PawnKindDef OG_Snotling;

        public static PawnKindDef Tribesperson_OG_Ork;
        public static PawnKindDef Refugee_FeralOrk;
        public static PawnKindDef StrangerInBlack_FeralOrk;
        public static PawnKindDef OG_Ork_Wild;

        public static PawnKindDef Tribesperson_OG_Grot;
        public static PawnKindDef Refugee_FeralGrot;
        public static PawnKindDef StrangerInBlack_FeralGrot;
        public static PawnKindDef OG_Grot_Wild;
    }
}
