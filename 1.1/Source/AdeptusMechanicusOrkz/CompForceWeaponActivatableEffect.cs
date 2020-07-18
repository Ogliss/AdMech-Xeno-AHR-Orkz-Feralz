using Verse;
using RimWorld;
using FeralOrkz;
using System.Collections.Generic;
using FeralOrkz.ExtensionMethods;

namespace FeralOrkz
{
    public class CompProperties_ForceWeaponActivatableEffect : CompProperties_AlwaysActivatableEffect
    {
        public CompProperties_ForceWeaponActivatableEffect() => this.compClass = typeof(CompForceWeaponActivatableEffect);
        public bool PowerWeapon = false;
        public bool ForceEffectRequiresPsyker = true;
        public DamageDef ForceWeaponEffect = null;
        public HediffDef ForceWeaponHediff = null;
        public float ForceWeaponKillChance = 0f;
        public SoundDef ForceWeaponTriggerSound = null;

    }

    public class CompForceWeaponActivatableEffect : CompAlwaysActivatableEffect
    {

        private OgsCompActivatableEffect.CompActivatableEffect.State currentState = OgsCompActivatableEffect.CompActivatableEffect.State.Deactivated;
        public new CompProperties_ForceWeaponActivatableEffect Props => this.props as CompProperties_ForceWeaponActivatableEffect;

        public bool ForceEffectRequiresPsyker
        {
            get
            {
                return Props.ForceEffectRequiresPsyker;
            }
        }
        public DamageDef ForceWeaponEffect
        {
            get
            {
                return Props.ForceWeaponEffect;
            }
        }
        public HediffDef ForceWeaponHediff
        {
            get
            {
                return Props.ForceWeaponHediff;
            }
        }
        public float ForceWeaponKillChance
        {
            get
            {
                return Props.ForceWeaponKillChance;
            }
        }
        public SoundDef ForceWeaponTriggerSound
        {
            get
            {
                return Props.ForceWeaponTriggerSound;
            }
        }

        public CompEquippable Equippable
        {
            get
            {
                return this.parent.TryGetComp<CompEquippable>() ?? null;
            }
        }

        public override bool CanActivate()
        {
            if (Equippable == null)
            {
                return false;
            }
            /*
            if (specialRules == null)
            {
                Log.Warning(parent.LabelCap+ " is a Force weapons without a specialRules comp");
                return false;
            }
            */
            if (Equippable.PrimaryVerb == null)
            {
                return false;
            }
            if (Equippable.PrimaryVerb.CasterPawn == null)
            {
                return false;
            }
            Pawn p = Equippable.PrimaryVerb.CasterPawn;
            //    Log.Message(string.Format("{0} CanActivate IsFighting: {1}, lastGivenWorkType: {2}, alwaysShowWeapon: {3}", GetPawn.LabelShortCap, GetPawn.IsFighting(), GetPawn.mindState.lastGivenWorkType, GetPawn.CurJobDef.alwaysShowWeapon));
            if (!p.Spawned || p.Map == null)
            {
                return false;
            }
            if (Props.ForceEffectRequiresPsyker)
            {
                if (!p.isPsyker())
                {
                    return false;
                }
            }
            return base.CanActivate();
        }


        public override void Initialize()
        {
            base.Initialize();
            if (GetPawn != null && GetPawn.isPsyker(out int level))
            {
                this.currentState = OgsCompActivatableEffect.CompActivatableEffect.State.Activated;
            }
        }

        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override string GetDescriptionPart()
        {

            string str = string.Empty;
            CompEquippable c = parent.GetComp<CompEquippable>();
            if (ForceWeapon)
            {
                List<Tool> list = parent.def.tools.FindAll(x => x.capacities.Any(y => y.defName.Contains("OG_ForceWeapon_")));
                List<string> listl = new List<string>();
                list.ForEach(x => listl.Add(x.label));
                str = str + string.Format("\n Force Weapon: Attacks made by the following Tools can cause Force Attacks if the wielder is a Psyker:\n{0}", listl.ToCommaList(), Props.ForceWeaponKillChance);
            }
            return str;
        }
    }
}