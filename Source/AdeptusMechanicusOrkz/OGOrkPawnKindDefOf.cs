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

        public static PawnKindDef Squig;

        public static PawnKindDef Snotling;

        public static PawnKindDef WildGrot;

        public static PawnKindDef WildOrk;
    }
}
