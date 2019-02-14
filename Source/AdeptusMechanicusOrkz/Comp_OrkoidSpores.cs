using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus
{

    public class CompProperties_OrkoidSpores : CompProperties
    {
        public CompProperties_OrkoidSpores()
        {
            this.compClass = typeof(Comp_OrkoidSpores);
        }

        public bool canspawn = true;
        public bool spawnwild = true;
        public float spawnChance = 0.05f;
        public float snotlingChance = 0.035f;
        public float grotChance = 0.015f;
        public float orkChance = 0.005f;
    }

    public class Comp_OrkoidSpores : ThingComp
    {
        public CompProperties_OrkoidSpores Props
        {
            get
            {
                return (CompProperties_OrkoidSpores)this.props;
            }
        }

        public override void PostDeSpawn(Map map)
        {
            Faction faction;
            PawnGenerationContext generationContext;
            bool canspawn = Props.canspawn;
            bool spawnwild = Props.spawnwild;
            float spawnChance = Props.spawnChance;
            float snotlingChance = Props.snotlingChance;
            float grotChance = Props.grotChance;
            float orkChance = Props.orkChance;
            int age = 0;
            PawnKindDef pawnKindDef;
            base.PostDeSpawn(map);
        //   Log.Message(string.Format("canspawn {0}", canspawn));
            if (canspawn == true)
            {
                var spawnRoll = Rand.Value;
            //   Log.Message(string.Format("rolled {0} needs less than {1} to spawn", spawnRoll, spawnChance));
                if (spawnRoll < spawnChance)
                {
                    spawnRoll = Rand.Value;
                    if (spawnRoll < snotlingChance & spawnRoll > grotChance)
                    {
                        pawnKindDef = OGOrkPawnKindDefOf.Snotling;
                    }
                    else if (spawnRoll < grotChance & spawnRoll > orkChance)
                    {
                        pawnKindDef = OGOrkPawnKindDefOf.WildGrot;
                        age = 16;
                    }
                    else if (spawnRoll < orkChance)
                    {
                        pawnKindDef = OGOrkPawnKindDefOf.WildOrk;
                        age = 16;
                    }
                    else
                    {
                        pawnKindDef = OGOrkPawnKindDefOf.Squig;
                    }
                //   Log.Message(string.Format("{0}", pawnKindDef));
                    if (spawnwild)
                    {
                        faction = null;
                        generationContext = PawnGenerationContext.NonPlayer;
                    }
                    else
                    {
                        faction = Faction.OfPlayer;
                        generationContext = PawnGenerationContext.PlayerStarter;
                    }
                    PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(pawnKindDef, faction, generationContext, -1, true, false, false, false, true, true, 20f, fixedGender: Gender.Male, fixedBiologicalAge: age, fixedChronologicalAge: age);
                    Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
                    //pawn.ageTracker.AgeBiologicalTicks = 70000000L;
                    if (spawnwild && pawnKindDef != OGOrkPawnKindDefOf.Snotling && pawnKindDef != OGOrkPawnKindDefOf.Squig)
                    {
                    //   Log.Message(string.Format("changing {0} to wildman", pawnKindDef));
                        pawn.ChangeKind(PawnKindDefOf.WildMan);
                    }
                    else if (!spawnwild && Faction.OfPlayer.def == OGOrkFactionDefOf.OrkPlayerColonyTribal && pawnKindDef != OGOrkPawnKindDefOf.Snotling && pawnKindDef != OGOrkPawnKindDefOf.Squig)
                    {
                    //   Log.Message(string.Format("changing {0} to colonist", pawnKindDef));
                        pawn.ChangeKind(PawnKindDefOf.Colonist);
                    }
                    GenSpawn.Spawn(pawn, base.parent.Position, map, 0);
                }
            }
        }

    }
 
}
