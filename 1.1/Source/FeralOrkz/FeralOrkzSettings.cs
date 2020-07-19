using System.Collections.Generic;
using Verse;

namespace FeralOrkz
{
    public class FeralOrkzSettings : ModSettings
    {
        public float FungusSpawnChance = 0.05f;
        public string FungusSpawnChanceBuffer;
        public float FungusSquigChance = 1f;
        public string FungusSquigChanceBuffer;
        public float FungusSnotChance = 0.35f;
        public string FungusSnotChanceBuffer;
        public float FungusGrotChance = 0.15f;
        public string FungusGrotChanceBuffer;
        public float FungusOrkChance = 0.075f;
        public string FungusOrkChanceBuffer;

        public float CocoonSpawnChance = 0.25f;
        public string CocoonSpawnChanceBuffer;
        public float CocoonSquigChance = 1f;
        public string CocoonSquigChanceBuffer;
        public float CocoonSnotChance = 0.5f;
        public string CocoonSnotChanceBuffer;
        public float CocoonGrotChance = 0.25f;
        public string CocoonGrotChanceBuffer;
        public float CocoonOrkChance = 0.15f;
        public string CocoonOrkChanceBuffer;

        public FeralOrkzSettings()
        {
            FeralOrkzSettings.Instance = this;
        }

        public static FeralOrkzSettings Instance;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.FungusSpawnChance, "AMO_FungusSpawnChance", 0.05f);
            Scribe_Values.Look(ref this.FungusSpawnChanceBuffer, "AMO_FungusSpawnChanceBuffer", string.Empty);
            Scribe_Values.Look(ref this.FungusSnotChance, "AMO_FungusSnotChance", 0.35f);
            Scribe_Values.Look(ref this.FungusSnotChanceBuffer, "AMO_FungusSnotChanceBuffer", string.Empty);
            Scribe_Values.Look(ref this.FungusGrotChance, "AMO_FungusGrotChance", 0.15f);
            Scribe_Values.Look(ref this.FungusGrotChanceBuffer, "AMO_FungusGrotChanceBuffer", string.Empty);
            Scribe_Values.Look(ref this.FungusOrkChance, "AMO_FungusOrkChance", 0.075f);
            Scribe_Values.Look(ref this.FungusOrkChanceBuffer, "AMO_FungusOrkChanceBuffer", string.Empty);

            Scribe_Values.Look(ref this.CocoonSpawnChance, "AMO_CocoonSpawnChance", 0.25f);
            Scribe_Values.Look(ref this.CocoonSpawnChanceBuffer, "AMO_CocoonSpawnChanceBuffer", string.Empty);
            Scribe_Values.Look(ref this.CocoonSnotChance, "AMO_CocoonSnotChance", 0.5f);
            Scribe_Values.Look(ref this.CocoonSnotChanceBuffer, "AMO_CocoonSnotChanceBuffer", string.Empty);
            Scribe_Values.Look(ref this.CocoonGrotChance, "AMO_CocoonGrotChance", 0.25f);
            Scribe_Values.Look(ref this.CocoonGrotChanceBuffer, "AMO_CocoonGrotChanceBuffer", string.Empty);
            Scribe_Values.Look(ref this.CocoonOrkChance, "AMO_CocoonOrkChance", 0.15f);
            Scribe_Values.Look(ref this.CocoonOrkChanceBuffer, "AMO_CocoonOrkChanceBuffer", string.Empty);

        }
    }
}
