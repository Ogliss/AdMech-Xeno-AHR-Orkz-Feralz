using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace FeralOrkz.ExtensionMethods
{
    public static class DamageDefExtensions
    {
        public static bool forceWeapon(this DamageDef damage)
        {
            return damage.defName.Contains("OG_Force_");
        }
    }
}
