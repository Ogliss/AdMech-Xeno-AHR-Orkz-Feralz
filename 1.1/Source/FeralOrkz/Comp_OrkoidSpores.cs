using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlienRace;

namespace FeralOrkz
{

    public class CompProperties_OrkoidSpores : CompProperties
    {
        public CompProperties_OrkoidSpores()
        {
            this.compClass = typeof(Comp_OrkoidSpores);
        }

        public bool canspawn = true;
        public bool spawnwild = true;
        public float spawnChance = 0.075f;
        public float snotlingChance = 0.035f;
        public float grotChance = 0.025f;
        public float orkChance = 0.015f;
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

        public Plant plant => base.parent as Plant;

        public bool canspawn => plant.HarvestableNow && Props.canspawn;

        public bool spawnwild => Props.spawnwild;
        public float spawnChance => parent.def.defName.Contains("Cocoon") ? FeralOrkzMod.Instance.settings.CocoonSpawnChance : FeralOrkzMod.Instance.settings.FungusSpawnChance;

        public float snotlingChance => parent.def.defName.Contains("Cocoon") ? FeralOrkzMod.Instance.settings.CocoonSnotChance : FeralOrkzMod.Instance.settings.FungusSnotChance;
        public float grotChance => parent.def.defName.Contains("Cocoon") ? FeralOrkzMod.Instance.settings.CocoonGrotChance : FeralOrkzMod.Instance.settings.FungusGrotChance;
        public float orkChance => parent.def.defName.Contains("Cocoon") ? FeralOrkzMod.Instance.settings.CocoonOrkChance : FeralOrkzMod.Instance.settings.FungusOrkChance;

        private float age = 0;
        public float Age
        {
            get
            {
                if (plant != null)
                {
                    age = plant.Age;
                }
                return age;
            }
        }
        private float fertility = 0;
        public float Fertility
        {
            get
            {
                if (plant != null)
                {
                    if (plant.Map != null)
                    {
                        fertility = plant.GrowthRateFactor_Fertility;
                    }
                }
                return fertility;
            }
        }

        public List<Pair<PawnKindDef, float>> pairs
        {
            get
            {
                float animalschance = HealthTuning.DeathOnDownedChance_NonColonyHumanlikeFromPopulationIntentCurve.Evaluate(Pawns.Count()) * Find.Storyteller.difficulty.enemyDeathOnDownedChanceFactor;
                float chance = HealthTuning.DeathOnDownedChance_NonColonyHumanlikeFromPopulationIntentCurve.Evaluate(StorytellerUtilityPopulation.PopulationIntent) * Find.Storyteller.difficulty.enemyDeathOnDownedChanceFactor;
                return new List<Pair<PawnKindDef, float>>()
                {
                    new Pair<PawnKindDef, float>(OGOrkPawnKindDefOf.OG_Squig, 1f * animalschance),
                    new Pair<PawnKindDef, float>(OGOrkPawnKindDefOf.OG_Snotling, snotlingChance * animalschance),
                    new Pair<PawnKindDef, float>(OGOrkPawnKindDefOf.OG_Grot_Wild, grotChance * chance),
                    new Pair<PawnKindDef, float>(OGOrkPawnKindDefOf.OG_Ork_Wild, orkChance * chance)
                };
            }
        }

        protected IEnumerable<Pawn> Pawns
        {
            get
            {
                return from p in Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer)
                       where p.RaceProps.Animal
                       select p;
            }
        }
        public override void PostDeSpawn(Map map)
        {
            if (canspawn)
            {
                var spawnRoll = Rand.Value;
                if (spawnRoll < (spawnChance * plant.Growth))
                {
                    pawnKindDef = pairs.RandomElementByWeight(x => x.Second).First;
                    faction = spawnwild ? null : Faction.OfPlayer;
                    generationContext = spawnwild ? PawnGenerationContext.NonPlayer : PawnGenerationContext.NonPlayer;
                    PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(pawnKindDef, faction, generationContext, -1, true, true, false, false, true, true, 0f, fixedGender: Gender.None, fixedBiologicalAge: Age, fixedChronologicalAge: Age);

                    Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);

                    if (pawnKindDef.RaceProps.Humanlike)
                    {
                        /*
                        if (pawn.kindDef == OGOrkPawnKindDefOf.OG_Ork_Wild)
                        {
                            pawn.story.childhood.identifier = "Ork_Base_Child";
                        }
                        else if (pawn.kindDef == OGOrkPawnKindDefOf.OG_Grot_Wild)
                        {
                            pawn.story.childhood.identifier = "Grot_Base_Child";
                        }
                        */
                        if (!spawnwild && (Faction.OfPlayer.def == OGOrkFactionDefOf.OG_Ork_PlayerTribe || Faction.OfPlayer.def == OGOrkFactionDefOf.OG_Grot_PlayerTribe))
                        {
                            PawnKindDef pawnKind = pawn.def.defName.Contains("Alien_Grot") ? OGOrkPawnKindDefOf.Tribesperson_OG_Grot : OGOrkPawnKindDefOf.Tribesperson_OG_Ork;
                            pawn.ChangeKind(pawnKind);
                        }
                        else
                        {
                            pawn.ChangeKind(PawnKindDefOf.WildMan);
                        }
                        pawn.story.bodyType = pawn.story.childhood.BodyTypeFor(pawn.gender);
                    }
                    if (Fertility < 1f)
                    {
                        foreach (Need need in pawn.needs.AllNeeds)
                        {
                            need.CurLevel = 0f;
                        }
                        Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.Malnutrition, pawn);
                        hediff.Severity = Math.Min(1f - Fertility, 0.75f);
                        pawn.health.AddHediff(hediff);
                    }
                    else
                    {
                        foreach (Need need in pawn.needs.AllNeeds)
                        {
                            need.CurLevel = Fertility - 1f;
                        }
                    }
                    GenSpawn.Spawn(pawn, base.parent.Position, map, 0);
                }
            }
            base.PostDeSpawn(map);
        }

        public PawnKindDef pawnKindDef;

        public Faction faction;

        public PawnGenerationContext generationContext;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.age, "PlantAge");
            Scribe_Values.Look(ref this.fertility, "PlantFertility");
        }
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (age == 0)
            {
                age = Age;
            }
            if (fertility == 0)
            {
                fertility = Fertility;
            }
        }
    }

}
