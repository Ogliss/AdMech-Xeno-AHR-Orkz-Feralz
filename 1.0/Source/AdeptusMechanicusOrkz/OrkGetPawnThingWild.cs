using System;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02000002 RID: 2
	public class OrkGetPawnThingWild : MoteThrown
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public override void Tick()
		{
			bool flag = base.Map == null;
			if (flag)
			{
				this.Destroy(0);
			}
            var spawnRoll = (Rand.Value*100);
            int spawnChance = 45;
            int snotlingChance = 65;
            int grotChance = 75;
            int orkChance = 90;
            if (spawnRoll>spawnChance)
            {
                PawnKindDef pawnKindDef = OGOrkPawnKindDefOf.Squig;
                if (spawnRoll > snotlingChance & spawnRoll < grotChance)
                {
                    pawnKindDef = OGOrkPawnKindDefOf.Snotling;
                }
                else if (spawnRoll > grotChance & spawnRoll < orkChance)
                {
                    pawnKindDef = OGOrkPawnKindDefOf.WildGrot;
                }
                else if (spawnRoll > orkChance)
                {
                    pawnKindDef = OGOrkPawnKindDefOf.WildOrk;
                }
                PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(pawnKindDef, null, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, true, 20f, fixedBiologicalAge: 15f);
                Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
                pawn.ageTracker.AgeBiologicalTicks = 70000000L;
                GenSpawn.Spawn(pawn, base.Position, base.Map, 0);
            }
            else
            {
                Thing thing = ThingMaker.MakeThing(OGOrkThingDefOf.Plant_OrkFungus, null);
                thing.stackCount = Rand.Range(1, 15);
                GenPlace.TryPlaceThing(thing, base.Position, base.Map, ThingPlaceMode.Near, null, null);
            }
			this.Destroy(0);
		}
	}
}
