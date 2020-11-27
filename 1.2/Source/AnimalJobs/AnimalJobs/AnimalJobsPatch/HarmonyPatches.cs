using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace AnimalJobsPatch
{
	// Token: 0x0200002F RID: 47
	[StaticConstructorOnStartup]
	internal static class HarmonyPatches
	{
		// Token: 0x060000E9 RID: 233 RVA: 0x000080E4 File Offset: 0x000062E4
		static HarmonyPatches()
		{
			Harmony harmony = new Harmony("rimworld.walkingproblem.animaljobspatch");
			MethodInfo original = AccessTools.Method(typeof(GenConstruct), "CanConstruct", null, null);
			MethodInfo original2 = AccessTools.Method(typeof(Toils_Haul), "JumpToCarryToNextContainerIfPossible", null, null);
			MethodInfo original3 = AccessTools.Method(typeof(Bill), "PawnAllowedToStartAnew", null, null);
			MethodInfo methodInfo = AccessTools.Method(typeof(Toils_Recipe), "DoRecipeWork", null, null);
			MethodInfo original4 = AccessTools.Method(typeof(Frame), "CompleteConstruction", null, null);
			MethodInfo methodInfo2 = AccessTools.Method(typeof(Thing), "Destroy", null, null);
			MethodInfo methodInfo3 = AccessTools.Method(typeof(FoodUtility), "TryFindBestFoodSourceFor", null, null);
			MethodInfo methodInfo4 = AccessTools.Method(typeof(Corpse), "ButcherProducts", null, null);
			MethodInfo original5 = AccessTools.Method(typeof(GenConstruct), "HandleBlockingThingJob", null, null);
			HarmonyMethod prefix = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CanConstruct_Prefix"));
			HarmonyMethod prefix2 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("JumpToCarryToNextContainerIfPossible_Prefix"));
			HarmonyMethod prefix3 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("PawnAllowedToStartAnew_Prefix"));
			HarmonyMethod harmonyMethod = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("DoRecipeWork_Prefix"));
			HarmonyMethod prefix4 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompleteConstruction_Prefix"));
			HarmonyMethod harmonyMethod2 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("Destroy_Prefix"));
			HarmonyMethod harmonyMethod3 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("TryFindBestFoodSourceFor_Prefix"));
			HarmonyMethod harmonyMethod4 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("ButcherProducts_Prefix"));
			HarmonyMethod prefix5 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("HandleBlockingThingJob_Prefix"));
			harmony.Patch(original, prefix, null, null, null);
			harmony.Patch(original2, prefix2, null, null, null);
			harmony.Patch(original3, prefix3, null, null, null);
			harmony.Patch(original4, prefix4, null, null, null);
			harmony.Patch(original5, prefix5, null, null, null);
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00008318 File Offset: 0x00006518
		public static void CanConstruct_Prefix(ref bool __result, Thing t, Pawn p, bool checkSkills = true, bool forced = false)
		{
			bool flag = !p.kindDef.RaceProps.Humanlike;
			if (flag)
			{
				bool flag2 = GenConstruct.FirstBlockingThing(t, p) != null;
				if (flag2)
				{
					__result = false;
				}
				LocalTargetInfo target = t;
				PathEndMode peMode = PathEndMode.Touch;
				Danger maxDanger = (!forced) ? p.NormalMaxDanger() : Danger.Deadly;
				bool flag3 = !p.CanReserveAndReach(target, peMode, maxDanger, 1, -1, null, forced);
				if (flag3)
				{
					__result = false;
				}
				bool flag4 = t.IsBurning();
				if (flag4)
				{
					__result = false;
				}
				__result = true;
			}
			__result = false;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x000083A0 File Offset: 0x000065A0
		public static void HandleBlockingThingJob_Prefix(ref Job __result, Thing constructible, Pawn worker, bool forced = false)
		{
			Thing thing = GenConstruct.FirstBlockingThing(constructible, worker);
			bool flag = thing == null;
			if (flag)
			{
				__result = null;
			}
			bool flag2 = thing.def.category == ThingCategory.Plant && worker.RaceProps.Animal;
			if (flag2)
			{
				bool flag3 = worker.CanReserveAndReach(thing, PathEndMode.ClosestTouch, worker.NormalMaxDanger(), 1, -1, null, forced);
				if (flag3)
				{
					__result = new Job(JobDefOf.CutPlant, thing);
				}
			}
			else
			{
				bool flag4 = thing.def.category == ThingCategory.Item && worker.RaceProps.Animal;
				if (flag4)
				{
					bool everHaulable = thing.def.EverHaulable;
					if (everHaulable)
					{
						__result = HaulAIUtility.HaulAsideJobFor(worker, thing);
					}
					Log.ErrorOnce(string.Concat(new object[]
					{
						"Never haulable ",
						thing,
						" blocking ",
						constructible.ToStringSafe<Thing>(),
						" at ",
						constructible.Position
					}), 6429262, false);
				}
				else
				{
					bool flag5 = thing.def.category == ThingCategory.Building && worker.RaceProps.Animal;
					if (flag5)
					{
						bool flag6 = worker.CanReserveAndReach(thing, PathEndMode.Touch, worker.NormalMaxDanger(), 1, -1, null, forced);
						if (flag6)
						{
							__result = new Job(WPJobDefOf.WPDeconstruct, thing)
							{
								ignoreDesignations = true
							};
						}
					}
				}
			}
			__result = null;
		}

		public static void JumpToCarryToNextContainerIfPossible_Prefix(ref Toil __result, Toil carryToContainerToil, TargetIndex primaryTargetInd)
		{
			Toil toil = new Toil();
			toil.initAction = delegate ()
			{
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				if (actor.carryTracker.CarriedThing != null)
				{
					LocalTargetInfo target;
					if (curJob.targetQueueB != null && curJob.targetQueueB.Count > 0 && actor.RaceProps.Animal)
					{
						target = curJob.GetTarget(primaryTargetInd);
						Thing primaryTarget2 = target.Thing;
						bool hasSpareItems2 = actor.carryTracker.CarriedThing.stackCount > GenConstruct.AmountNeededByOf((IConstructible)(object)(IConstructible)primaryTarget2, actor.carryTracker.CarriedThing.def);
						Predicate<Thing> predicate = (Thing th) => ReservationUtility.CanReserve(actor, primaryTarget2, 1, -1, (ReservationLayerDef)null, false) && GenCollection.Any<ThingDefCountClass>(((IConstructible)th).MaterialsNeeded(), (Predicate<ThingDefCountClass>)((ThingDefCountClass need) => need.thingDef == actor.carryTracker.CarriedThing.def)) && ((th == primaryTarget2) | hasSpareItems2);
						Thing nextTarget2 = GenClosest.ClosestThing_Global_Reachable(actor.Position, actor.Map, curJob.targetQueueB.Select(delegate (LocalTargetInfo targ)
						{
							//IL_0000: Unknown result type (might be due to invalid IL or missing references)
							//IL_0001: Unknown result type (might be due to invalid IL or missing references)
							LocalTargetInfo val2 = targ;
							return val2.Thing;
						}), (PathEndMode)2, TraverseParms.For(actor, (Danger)3, (TraverseMode)0, false), 99999f, predicate, (Func<Thing, float>)null);
						if (nextTarget2 != null)
						{
							curJob.targetQueueB.RemoveAll((LocalTargetInfo targ) => targ.Thing == nextTarget2);
							curJob.targetB = nextTarget2;
							actor.jobs.curDriver.JumpToToil(carryToContainerToil);
						}
					}
					if (curJob.targetQueueB != null && curJob.targetQueueB.Count > 0 && !actor.RaceProps.Animal)
					{
						target = curJob.GetTarget(primaryTargetInd);
						Thing primaryTarget = target.Thing;
						bool hasSpareItems = actor.carryTracker.CarriedThing.stackCount > GenConstruct.AmountNeededByOf((IConstructible)(object)(IConstructible)primaryTarget, actor.carryTracker.CarriedThing.def);
						Predicate<Thing> predicate2 = (Thing th) => GenConstruct.CanConstruct(th, actor, false, false) && GenCollection.Any<ThingDefCountClass>(((IConstructible)th).MaterialsNeeded(), (Predicate<ThingDefCountClass>)((ThingDefCountClass need) => need.thingDef == actor.carryTracker.CarriedThing.def)) && ((th == primaryTarget) | hasSpareItems);
						Thing nextTarget = GenClosest.ClosestThing_Global_Reachable(actor.Position, actor.Map, curJob.targetQueueB.Select(delegate (LocalTargetInfo targ)
						{
							//IL_0000: Unknown result type (might be due to invalid IL or missing references)
							//IL_0001: Unknown result type (might be due to invalid IL or missing references)
							LocalTargetInfo val = targ;
							return val.Thing;
						}), (PathEndMode)2, TraverseParms.For(actor, (Danger)3, (TraverseMode)0, false), 99999f, predicate2, (Func<Thing, float>)null);
						if (nextTarget != null)
						{
							curJob.targetQueueB.RemoveAll((LocalTargetInfo targ) => targ.Thing == nextTarget);
							curJob.targetB = nextTarget;
							actor.jobs.curDriver.JumpToToil(carryToContainerToil);
						}
					}
				}
			};
			__result = toil;
		}
		// Token: 0x060000ED RID: 237 RVA: 0x00008550 File Offset: 0x00006750
		public static void PawnAllowedToStartAnew_Prefix(ref bool __result, Pawn p)
		{
			bool animal = p.RaceProps.Animal;
			if (animal)
			{
				__result = true;
			}
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00008574 File Offset: 0x00006774
		public static void DoRecipeWork_Prefix(ref Toil __result)
		{
			Toil toil = new Toil();
			toil.initAction = delegate()
			{
				Log.Message("init delegated.", false);
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				bool animal = actor.RaceProps.Animal;
				if (animal)
				{
					Log.Message("I am in.", false);
					JobDriver curDriver = actor.jobs.curDriver;
					JobDriver_WPDoBill jobDriver_WPDoBill = curDriver as JobDriver_WPDoBill;
					UnfinishedThing unfinishedThing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
					jobDriver_WPDoBill.workLeft = curJob.bill.recipe.WorkAmountTotal(unfinishedThing.def);
					bool flag = unfinishedThing != null;
					if (flag)
					{
						unfinishedThing.workLeft = jobDriver_WPDoBill.workLeft;
					}
					jobDriver_WPDoBill.billStartTick = Find.TickManager.TicksGame;
					jobDriver_WPDoBill.ticksSpentDoingRecipeWork = 0;
					curJob.bill.Notify_DoBillStarted(actor);
				}
				else
				{
					Log.Message("I am here instead.", false);
					JobDriver_DoBill jobDriver_DoBill = (JobDriver_DoBill)actor.jobs.curDriver;
					UnfinishedThing unfinishedThing2 = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
					bool flag2 = unfinishedThing2 != null && unfinishedThing2.Initialized;
					if (flag2)
					{
						jobDriver_DoBill.workLeft = unfinishedThing2.workLeft;
					}
					else
					{
						jobDriver_DoBill.workLeft = curJob.bill.recipe.WorkAmountTotal((unfinishedThing2 == null) ? null : unfinishedThing2.Stuff);
						bool flag3 = unfinishedThing2 != null;
						if (flag3)
						{
							unfinishedThing2.workLeft = jobDriver_DoBill.workLeft;
						}
					}
					jobDriver_DoBill.billStartTick = Find.TickManager.TicksGame;
					jobDriver_DoBill.ticksSpentDoingRecipeWork = 0;
					curJob.bill.Notify_DoBillStarted(actor);
				}
			};
			toil.tickAction = delegate()
			{
				Log.Message("tick delegated.", false);
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				JobDriver curDriver = actor.jobs.curDriver;
				bool animal = actor.RaceProps.Animal;
				if (animal)
				{
					JobDriver_WPDoBill jobDriver_WPDoBill = curDriver as JobDriver_WPDoBill;
					UnfinishedThing unfinishedThing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
					bool flag = unfinishedThing != null && unfinishedThing.Destroyed;
					if (flag)
					{
						actor.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
						return;
					}
					jobDriver_WPDoBill.ticksSpentDoingRecipeWork++;
					curJob.bill.Notify_PawnDidWork(actor);
					IBillGiverWithTickAction billGiverWithTickAction = toil.actor.CurJob.GetTarget(TargetIndex.A).Thing as IBillGiverWithTickAction;
					bool flag2 = billGiverWithTickAction != null;
					if (flag2)
					{
						billGiverWithTickAction.UsedThisTick();
					}
					float num = (curJob.RecipeDef.workSpeedStat != null) ? actor.GetStatValue(curJob.RecipeDef.workSpeedStat, true) : 1f;
					Building_WorkTable building_WorkTable = jobDriver_WPDoBill.BillGiver as Building_WorkTable;
					bool flag3 = building_WorkTable != null;
					if (flag3)
					{
						num *= building_WorkTable.GetStatValue(StatDefOf.WorkTableWorkSpeedFactor, true);
					}
					bool fastCrafting = DebugSettings.fastCrafting;
					if (fastCrafting)
					{
						num *= 30f;
					}
					jobDriver_WPDoBill.workLeft -= num;
					bool flag4 = unfinishedThing != null;
					if (flag4)
					{
						unfinishedThing.workLeft = jobDriver_WPDoBill.workLeft;
					}
					actor.GainComfortFromCellIfPossible(false);
					bool flag5 = jobDriver_WPDoBill.workLeft <= 0f;
					if (flag5)
					{
						jobDriver_WPDoBill.ReadyForNextToil();
					}
					bool usesUnfinishedThing = curJob.bill.recipe.UsesUnfinishedThing;
					if (usesUnfinishedThing)
					{
						int num2 = Find.TickManager.TicksGame - jobDriver_WPDoBill.billStartTick;
						bool flag6 = num2 >= 3000 && num2 % 1000 == 0;
						if (flag6)
						{
							actor.jobs.CheckForJobOverride();
						}
					}
				}
				bool flag7 = !actor.RaceProps.Animal;
				if (flag7)
				{
					JobDriver_DoBill jobDriver_DoBill = (JobDriver_DoBill)actor.jobs.curDriver;
					UnfinishedThing unfinishedThing2 = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
					bool flag8 = unfinishedThing2 != null && unfinishedThing2.Destroyed;
					if (flag8)
					{
						actor.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
					}
					else
					{
						jobDriver_DoBill.ticksSpentDoingRecipeWork++;
						curJob.bill.Notify_PawnDidWork(actor);
						IBillGiverWithTickAction billGiverWithTickAction2 = toil.actor.CurJob.GetTarget(TargetIndex.A).Thing as IBillGiverWithTickAction;
						bool flag9 = billGiverWithTickAction2 != null;
						if (flag9)
						{
							billGiverWithTickAction2.UsedThisTick();
						}
						bool flag10 = curJob.RecipeDef.workSkill != null && curJob.RecipeDef.UsesUnfinishedThing;
						if (flag10)
						{
							actor.skills.GetSkill(curJob.RecipeDef.workSkill).Learn(0.11f * curJob.RecipeDef.workSkillLearnFactor, false);
						}
						float num3 = (curJob.RecipeDef.workSpeedStat != null) ? actor.GetStatValue(curJob.RecipeDef.workSpeedStat, true) : 1f;
						Building_WorkTable building_WorkTable2 = jobDriver_DoBill.BillGiver as Building_WorkTable;
						bool flag11 = building_WorkTable2 != null;
						if (flag11)
						{
							num3 *= building_WorkTable2.GetStatValue(StatDefOf.WorkTableWorkSpeedFactor, true);
						}
						bool fastCrafting2 = DebugSettings.fastCrafting;
						if (fastCrafting2)
						{
							num3 *= 30f;
						}
						jobDriver_DoBill.workLeft -= num3;
						bool flag12 = unfinishedThing2 != null;
						if (flag12)
						{
							unfinishedThing2.workLeft = jobDriver_DoBill.workLeft;
						}
						actor.GainComfortFromCellIfPossible(false);
						bool flag13 = jobDriver_DoBill.workLeft <= 0f;
						if (flag13)
						{
							jobDriver_DoBill.ReadyForNextToil();
						}
						bool usesUnfinishedThing2 = curJob.bill.recipe.UsesUnfinishedThing;
						if (usesUnfinishedThing2)
						{
							int num4 = Find.TickManager.TicksGame - jobDriver_DoBill.billStartTick;
							bool flag14 = num4 >= 3000 && num4 % 1000 == 0;
							if (flag14)
							{
								actor.jobs.CheckForJobOverride();
							}
						}
					}
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			toil.WithEffect(() => toil.actor.CurJob.bill.recipe.effectWorking, TargetIndex.A);
			toil.PlaySustainerOrSound(() => toil.actor.CurJob.bill.recipe.soundWorking);
			toil.WithProgressBar(TargetIndex.A, delegate
			{
				Pawn actor = toil.actor;
				Job curJob = actor.CurJob;
				UnfinishedThing unfinishedThing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
				bool animal = actor.RaceProps.Animal;
				float result;
				if (animal)
				{
					result = 1f - ((JobDriver_WPDoBill)actor.jobs.curDriver).workLeft / curJob.bill.recipe.WorkAmountTotal((unfinishedThing == null) ? null : unfinishedThing.Stuff);
				}
				else
				{
					result = 1f - ((JobDriver_DoBill)actor.jobs.curDriver).workLeft / curJob.bill.recipe.WorkAmountTotal((unfinishedThing == null) ? null : unfinishedThing.Stuff);
				}
				return result;
			}, false, -0.5f);
			toil.FailOn(() => toil.actor.CurJob.bill.suspended);
			__result = toil;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00008640 File Offset: 0x00006840
		public static bool CompleteConstruction_Prefix(Frame __instance, Pawn worker)
		{
			bool flag = !worker.RaceProps.Humanlike;
			bool result;
			if (flag)
			{
				Thing thing = __instance.holdingOwner.Take(__instance);
				__instance.resourceContainer.ClearAndDestroyContents(DestroyMode.Vanish);
				Map map = worker.Map;
				__instance.Destroy(DestroyMode.Vanish);
				bool flag2 = __instance.GetStatValue(StatDefOf.WorkToBuild, true) > 150f && __instance.def.entityDefToBuild is ThingDef && ((ThingDef)__instance.def.entityDefToBuild).category == ThingCategory.Building;
				if (flag2)
				{
					SoundDefOf.Building_Complete.PlayOneShot(new TargetInfo(thing.Position, map, false));
				}
				ThingDef thingDef = __instance.def.entityDefToBuild as ThingDef;
				Thing thing2 = null;
				bool flag3 = thingDef != null;
				if (flag3)
				{
					thing2 = ThingMaker.MakeThing(thingDef, thing.Stuff);
					thing2.SetFactionDirect(thing.Faction);
					CompQuality compQuality = thing2.TryGetComp<CompQuality>();
					bool flag4 = compQuality != null;
					if (flag4)
					{
						int relevantSkillLevel = 1;
						QualityCategory q = QualityUtility.GenerateQualityCreatedByPawn(relevantSkillLevel, false);
						compQuality.SetQuality(q, ArtGenerationContext.Colony);
						QualityUtility.SendCraftNotification(thing2, worker);
					}
					CompArt compArt = thing2.TryGetComp<CompArt>();
					bool flag5 = compArt != null;
					if (flag5)
					{
						bool flag6 = compQuality == null;
						if (flag6)
						{
							compArt.InitializeArt(ArtGenerationContext.Colony);
						}
						compArt.JustCreatedBy(worker);
					}
					thing2.HitPoints = Mathf.CeilToInt((float)__instance.HitPoints / (float)thing.MaxHitPoints * (float)thing2.MaxHitPoints);
					GenSpawn.Spawn(thing2, thing.Position, worker.Map, thing.Rotation, WipeMode.Vanish, false);
				}
				else
				{
					map.terrainGrid.SetTerrain(thing.Position, (TerrainDef)__instance.def.entityDefToBuild);
					FilthMaker.RemoveAllFilth(thing.Position, map);
				}
				worker.records.Increment(RecordDefOf.ThingsConstructed);
				bool flag7 = thing2 != null && thing2.GetStatValue(StatDefOf.WorkToBuild, true) >= 9500f;
				if (flag7)
				{
					TaleRecorder.RecordTale(TaleDefOf.CompletedLongConstructionProject, new object[]
					{
						worker,
						thing2.def
					});
				}
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00008874 File Offset: 0x00006A74
		public static void Destroy_Prefix(Thing __instance, DestroyMode mode = DestroyMode.Vanish)
		{
			bool flag = !Thing.allowDestroyNonDestroyable && !__instance.def.destroyable;
			if (!flag)
			{
				bool destroyed = __instance.Destroyed;
				if (!destroyed)
				{
					bool spawned = __instance.Spawned;
					Map map = __instance.Map;
					bool spawned2 = __instance.Spawned;
					if (spawned2)
					{
						__instance.DeSpawn(mode);
					}
					sbyte value = Traverse.Create(typeof(Thing)).Field("mapIndexOrState").GetValue<sbyte>();
					bool discardOnDestroyed = __instance.def.DiscardOnDestroyed;
					if (discardOnDestroyed)
					{
						__instance.Discard(false);
					}
					CompExplosive compExplosive = __instance.TryGetComp<CompExplosive>();
					bool flag2 = compExplosive != null && compExplosive.destroyedThroughDetonation;
					bool flag3 = spawned && !flag2;
					if (flag3)
					{
						GenLeaving.DoLeavingsFor(__instance, map, mode, null);
					}
					bool flag4 = __instance.holdingOwner != null;
					if (flag4)
					{
						__instance.holdingOwner.Notify_ContainedItemDestroyed(__instance);
					}
					List<Map> maps = Find.Maps;
					for (int i = 0; i < maps.Count; i++)
					{
						bool flag5 = __instance.def.category == ThingCategory.Mote;
						if (flag5)
						{
							return;
						}
						bool flag6 = __instance.def.category != ThingCategory.Mote;
						if (flag6)
						{
							maps[i].reservationManager.ReleaseAllForTarget(__instance);
							maps[i].physicalInteractionReservationManager.ReleaseAllForTarget(__instance);
							IAttackTarget attackTarget = __instance as IAttackTarget;
							bool flag7 = attackTarget != null;
							if (flag7)
							{
								maps[i].attackTargetReservationManager.ReleaseAllForTarget(attackTarget);
							}
							maps[i].designationManager.RemoveAllDesignationsOn(__instance, false);
						}
					}
					bool flag8 = !(__instance is Pawn);
					if (flag8)
					{
						__instance.stackCount = 0;
					}
				}
			}
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00008A58 File Offset: 0x00006C58
		public static void TryFindBestFoodSourceFor_Prefix(ref bool __result, Pawn getter, Pawn eater, bool desperate, out Thing foodSource, out ThingDef foodDef, bool canRefillDispenser = true, bool canUseInventory = true, bool allowForbidden = false, bool allowCorpse = true, bool allowSociallyImproper = false, bool allowHarvest = false)
		{
			bool flag = getter.RaceProps.ToolUser && getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
			bool animal = getter.RaceProps.Animal;
			bool flag2 = !eater.IsTeetotaler();
			Thing thing = null;
			if (canUseInventory)
			{
				bool flag3 = animal;
				if (flag3)
				{
					thing = FoodUtility.BestFoodInInventory(getter, eater, FoodPreferability.MealAwful, FoodPreferability.MealLavish, 0f, false);
				}
			}
			bool flag4 = getter == eater;
			ThingDef thingDef;
			Thing thing2 = FoodUtility.BestFoodSourceOnMap(getter, eater, desperate, out thingDef, FoodPreferability.MealLavish, flag4, flag2, allowCorpse, true, canRefillDispenser, allowForbidden, allowSociallyImproper, allowHarvest, false);
			bool flag5 = thing == null && thing2 == null;
			if (flag5)
			{
				bool flag6 = canUseInventory && animal;
				if (flag6)
				{
					FoodPreferability minFoodPref = FoodPreferability.DesperateOnly;
					bool allowDrug = flag2;
					thing = FoodUtility.BestFoodInInventory(getter, eater, minFoodPref, FoodPreferability.MealLavish, 0f, allowDrug);
					bool flag7 = thing != null;
					if (flag7)
					{
						foodSource = thing;
						foodDef = FoodUtility.GetFinalIngestibleDef(foodSource, false);
						__result = true;
					}
				}
				foodSource = null;
				foodDef = null;
				__result = false;
			}
			bool flag8 = thing == null && thing2 != null;
			if (flag8)
			{
				foodSource = thing2;
				foodDef = thingDef;
				__result = true;
			}
			ThingDef finalIngestibleDef = FoodUtility.GetFinalIngestibleDef(thing, false);
			bool flag9 = thing2 == null;
			if (flag9)
			{
				foodSource = thing;
				foodDef = finalIngestibleDef;
				__result = true;
			}
			float num = FoodUtility.FoodOptimality(eater, thing2, thingDef, (float)(getter.Position - thing2.Position).LengthManhattan, false);
			float num2 = FoodUtility.FoodOptimality(eater, thing, finalIngestibleDef, 0f, false);
			num2 -= 32f;
			bool flag10 = num > num2;
			if (flag10)
			{
				foodSource = thing2;
				foodDef = thingDef;
				__result = true;
			}
			foodSource = thing;
			foodDef = FoodUtility.GetFinalIngestibleDef(foodSource, false);
			__result = true;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00008C04 File Offset: 0x00006E04
		public static bool ButcherProducts_Prefix(Pawn butcher, float efficiency, ref IEnumerable<Thing> __result, Corpse __instance)
		{
			if (butcher.RaceProps.Animal)
			{
				__result = ((Func<IEnumerable<Thing>>)delegate
				{
					Log.Message("butcherproduct patched", false);
					Pawn innerPawn = __instance.InnerPawn;
					return ((Thing)innerPawn).ButcherProducts(butcher, efficiency);
				})();
				return true;
			}
			return true;
		}


		// Token: 0x0400003B RID: 59
		private static readonly Type patchType = typeof(HarmonyPatches);
	}
}
