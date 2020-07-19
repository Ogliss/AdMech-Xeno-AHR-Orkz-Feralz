using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace FeralOrkz.ExtensionMethods
{
    public static class PawnExtensions
    {
        public static bool isAdult(this Pawn pawn)
        {
            return pawn.RaceProps.lifeStageAges.Any(x => x.def.reproductive) && pawn.ageTracker.AgeBiologicalYearsFloat >= pawn.RaceProps.lifeStageAges.First(x => x.def.reproductive).minAge;
        }

        public static bool isPsyker(this Pawn pawn)
        {
            return pawn.isPsyker(out int Level);
        }

        public static bool isPsyker(this Pawn pawn, out int Level)
        {
            return pawn.isPsyker(out Level, out float Mult);
        }

        public static bool isPsyker(this Pawn pawn, out int Level, out float Mult)
        {
            bool result = false;
            Mult = 0f;
            Level = 0;

            if (pawn.RaceProps.Humanlike)
            {
                if (pawn.health.hediffSet.hediffs.Any(x => x.GetType() == typeof(Hediff_ImplantWithLevel)))
                {
                    Level = (pawn.health.hediffSet.hediffs.First(x => x.GetType() == typeof(Hediff_ImplantWithLevel)) as Hediff_ImplantWithLevel).level;
                    result = true;
                }
                else
                if (pawn.story.traits.HasTrait(TraitDefOf.PsychicSensitivity))
                {
                    result = pawn.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity) > 0;
                    Level = pawn.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity);
                }
                else
                {
                    TraitDef Corruptionpsyker = DefDatabase<TraitDef>.GetNamedSilentFail("Psyker");
                    if (Corruptionpsyker != null)
                    {
                        result = true;
                        pawn.story.traits.HasTrait(Corruptionpsyker);
                        Level = pawn.story.traits.DegreeOfTrait(Corruptionpsyker);
                    }
                }
                Mult = pawn.GetStatValue(StatDefOf.PsychicSensitivity) * (pawn.needs.mood.CurInstantLevelPercentage - pawn.health.hediffSet.PainTotal);
            }
            else
            {
                ToolUserPskyerDefExtension extension = null;
                if (pawn.def.HasModExtension<ToolUserPskyerDefExtension>())
                {
                    extension = pawn.def.GetModExtension<ToolUserPskyerDefExtension>();
                }
                else
                if (pawn.kindDef.HasModExtension<ToolUserPskyerDefExtension>())
                {
                    extension = pawn.kindDef.GetModExtension<ToolUserPskyerDefExtension>();
                }
                if (extension != null)
                {
                    result = true;
                    Level = extension.Level;
                }
                if (pawn.needs != null && pawn.needs.mood != null)
                {
                    Mult = pawn.GetStatValue(StatDefOf.PsychicSensitivity) * (pawn.needs.mood.CurInstantLevelPercentage - pawn.health.hediffSet.PainTotal);
                }
                else
                {
                    Mult = pawn.GetStatValue(StatDefOf.PsychicSensitivity) * (1 - pawn.health.hediffSet.PainTotal);
                }
            }

            return result;
        }

    }
}
