using System;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02000002 RID: 2
	public class OrkGetPawnThing : MoteThrown
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
            int spawnChance = 50;
            int snotlingChance = 65;
            int grotChance = 75;
            int orkChance = 90;
            string log = string.Format("rolled {0} needs over {1} to spawn", spawnRoll, spawnChance);
            Log.Message(log);
            if (spawnRoll>spawnChance)
            {
                PawnKindDef pawnKindDef = OGOrkPawnKindDefOf.Squig;

                if (spawnRoll > snotlingChance & spawnRoll < grotChance)
                {
                    pawnKindDef = OGOrkPawnKindDefOf.Snotling;
                }
                else if(spawnRoll > grotChance & spawnRoll < orkChance)
                {
                    pawnKindDef = OGOrkPawnKindDefOf.WildGrot;
                }
                else if (spawnRoll > orkChance)
                {
                    pawnKindDef = OGOrkPawnKindDefOf.WildOrk;
                }
                log = string.Format("{0}", pawnKindDef);
                Log.Message(log);

                PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(pawnKindDef, null, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, true, 20f);
                Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
                pawn.ageTracker.AgeBiologicalTicks = 70000000L;
                //pawn.ChangeKind(PawnKindDefOf.WildMan);
                GenSpawn.Spawn(pawn, base.Position, base.Map, 0);
            }
			this.Destroy(0);
		}
	}
}
