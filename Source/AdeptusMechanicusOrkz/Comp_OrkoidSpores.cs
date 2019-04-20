using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using AlienRace;

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

        public Plant plant
        {
            get
            {
                return base.parent as Plant;
            }
        }

        public bool canspawn
        {
            get
            {
                return plant.HarvestableNow && Props.canspawn;
            }
        }

        public bool spawnwild
        {
            get
            {
                return Props.spawnwild;
            }
        }

        public float spawnChance
        {
            get
            {
                return Props.spawnChance;
            }
        }

        public float snotlingChance
        {
            get
            {
                return Props.snotlingChance;
            }
        }

        public float grotChance
        {
            get
            {
                return Props.grotChance;
            }
        }

        public float orkChance
        {
            get
            {
                return Props.orkChance;
            }
        }

        public float age
        {
            get
            {
                if (pawnKindDef.RaceProps.Humanlike)
                {
                    return 16.5f;
                }
                else return 0f;
            }
            set
            {

            }
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
        //   Log.Message(string.Format("canspawn {0}", canspawn));
            if (canspawn == true)
            {
                var spawnRoll = Rand.Value;
            //   Log.Message(string.Format("rolled {0} needs less than {1} to spawn", spawnRoll, spawnChance));
                if (spawnRoll < (spawnChance*plant.Growth))
                {
                    spawnRoll = Rand.Value;
                    if (spawnRoll < snotlingChance & spawnRoll > grotChance)
                    {
                        pawnKindDef = OGOrkPawnKindDefOf.Snotling;
                    }
                    else if (spawnRoll < grotChance & spawnRoll > orkChance)
                    {
                        pawnKindDef = OGOrkPawnKindDefOf.WildGrot;
                        //age = 16;
                    }
                    else if (spawnRoll < orkChance)
                    {
                        pawnKindDef = OGOrkPawnKindDefOf.WildOrk;
                        //age = 16;
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
                    if (pawnKindDef.RaceProps.Humanlike)
                    {
                     //   age = pawnKindDef.RaceProps.lifeStageAges.First(x => x.def.defName.Contains("Adult")).minAge - 0.5f;
                    }
                    PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(pawnKindDef, faction, generationContext, -1, true, false, false, false, true, true, 20f, fixedGender: Gender.Male, fixedBiologicalAge: age, fixedChronologicalAge: age);
                    Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
                    
                    if (pawn.kindDef==OGOrkPawnKindDefOf.WildOrk)
                    {
                        pawn.story.childhood.identifier = "Ork_Base_Child";
                    }
                    else if (pawn.kindDef==OGOrkPawnKindDefOf.WildGrot)
                    {
                        pawn.story.childhood.identifier = "Grot_Base_Child";
                    }
                    if (spawnwild && pawnKindDef != OGOrkPawnKindDefOf.Snotling && pawnKindDef != OGOrkPawnKindDefOf.Squig)
                    {
                        //   Log.Message(string.Format("chang0ing {0} to wildman", pawnKindDef));
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

        public PawnKindDef pawnKindDef;

        public Faction faction;

        public PawnGenerationContext generationContext;
    }
 
}
