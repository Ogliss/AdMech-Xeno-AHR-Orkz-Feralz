using Verse;
using RimWorld;
using FeralOrkz;
using UnityEngine;
using System.Collections.Generic;
using OgsCompActivatableEffect;

namespace FeralOrkz
{
    public class CompProperties_AlwaysActivatableEffect : CompProperties_ActivatableEffect
    {
        public CompProperties_AlwaysActivatableEffect() => this.compClass = typeof(CompAlwaysActivatableEffect);
    }

    public class CompAlwaysActivatableEffect : CompActivatableEffect
    {

        private Graphic graphicInt;
        private OgsCompActivatableEffect.CompActivatableEffect.State currentState = OgsCompActivatableEffect.CompActivatableEffect.State.Deactivated;

        public bool PowerWeapon => parent.def.tools.Any(x => x.capacities.Any(y => y.defName.Contains("OG_PowerWeapon_")));
        public bool RendingWeapon => parent.def.tools.Any(x => x.capacities.Any(y => y.defName.Contains("OG_RendingWeapon_")));
        public bool ForceWeapon => parent.def.tools.Any(x => x.capacities.Any(y => y.defName.Contains("OG_ForceWeapon_")));
        public bool Witchblade => parent.def.tools.Any(x => x.capacities.Any(y => y.defName.Contains("OG_WitchbladeWeapon_")));
        public override bool CanActivate() => GetPawn != null && GetPawn.Spawned && GetPawn.Map != null;

        public string texPath
        {
            get
            {
                string tex = this.Props.graphicData.texPath;
                if (this.parent.TryGetComp<AdvancedGraphics.CompAdvancedGraphic>() != null && this.parent.TryGetComp<AdvancedGraphics.CompAdvancedGraphic>() is AdvancedGraphics.CompAdvancedGraphic graphic)
                {
                    tex = graphic.current.path;
                }
                if (tex.NullOrEmpty())
                {
                    tex = this.parent.def.graphicData.texPath;
                }
                return tex + "_Glow";
            }
        }

        private GraphicData intGraphicData;
        private GraphicData GraphicData
        {
            get
            {
                if (intGraphicData == null)
                {
                    intGraphicData = new GraphicData();
                    intGraphicData.CopyFrom(this.Props.graphicData);
                    intGraphicData.texPath = this.texPath;
                }
                return intGraphicData;
            }
        }

        public override Graphic Graphic
        {
            get
            {
                bool flag = this.graphicInt == null;
                if (flag)
                {
                    bool flag2 = this.Props.graphicData == null;
                    if (flag2)
                    {
                        Log.ErrorOnce(this.parent.def + " has no SecondLayer graphicData but we are trying to access it.", 764532, false);
                        return BaseContent.BadGraphic;
                    }
                    Color newColor = (GraphicData.color == Color.white) ? this.parent.DrawColor : GraphicData.color;
                    Color newColorTwo = (GraphicData.colorTwo == Color.white) ? this.parent.DrawColorTwo : GraphicData.colorTwo;
                    Shader shader = (GraphicData.shaderType == null) ? this.parent.Graphic.Shader : GraphicData.shaderType.Shader;
                    this.graphicInt = GraphicData.Graphic.GetColoredVersion(shader, newColor, newColorTwo);
                    this.graphicInt = this.PostGraphicEffects(this.graphicInt);
                }
                return this.graphicInt;
            }
            set
            {
                this.graphicInt = value;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            if (GetPawn != null && !GetPawn.IsColonist)
            {
                this.currentState = CompActivatableEffect.State.Activated;
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

        public override string CompInspectStringExtra()
        {
            string str = "Special Rules:";
            string str2 = string.Empty;
            if (ForceWeapon)
            {
                str2 = str2.NullOrEmpty() ? str + "Force Weapon" : str + ", Force Weapon";
            }
            return str2.NullOrEmpty() ? null : str + str2;
        }
    }
}