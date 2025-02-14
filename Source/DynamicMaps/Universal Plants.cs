﻿using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace DynamicMaps
{
	[HarmonyPatch(typeof(WildPlantSpawner), "CalculatePlantsWhichCanGrowAt")]
	internal static class UniversalPlants
	{
		internal static void Postfix(IntVec3 c, Map ___map, List<ThingDef> outPlants)
		{
			List<ThingDef> thingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
			foreach (ThingDef thingDef in thingDefs.Where(x => x.HasModExtension<DM_ModExtension>()))
			{
				outPlants.Remove(thingDef);
				DM_ModExtension ext = thingDef.GetModExtension<DM_ModExtension>();
				if (!thingDef.CanEverPlantAt_NewTemp(c, ___map))
				{
					return;
				}
				if (GenLocalDate.Season(___map) == ext.season || (SeasonUtility.GetPreviousSeason(ext.season) == GenLocalDate.Season(___map) && ext.spawnInPreviousSeason) || (DMSeasonUtility.GetNextSeason(ext.season) == GenLocalDate.Season(___map) && ext.spawnInNextSeason))
				{
					outPlants.Add(thingDef);
				}
			}
		}
	}
	[HarmonyPatch(typeof(WildPlantSpawner), "GetCommonalityOfPlant")]
	internal static class UniversalPlantsCommonality
	{
		internal static void Postfix(ref float __result, Map ___map, ThingDef plant)
		{
			if (!plant.HasModExtension<DM_ModExtension>())
			{
				return;
			}
			DM_ModExtension ext = plant.GetModExtension<DM_ModExtension>();
			if (SeasonUtility.GetPreviousSeason(ext.season) == GenLocalDate.Season(___map) && ext.spawnInPreviousSeason)
			{
				__result = (ext.commonality / 2);
				return;
			}
			__result = ext.commonality;
		}
	}
	public static class DMSeasonUtility
	{
		public static Season GetNextSeason(this Season season)
		{
			switch (season)
			{
				case Season.Undefined:
					return Season.Undefined;
				case Season.Spring:
					return Season.Summer;
				case Season.Summer:
					return Season.Fall;
				case Season.Fall:
					return Season.Winter;
				case Season.Winter:
					return Season.Spring;
				case Season.PermanentSummer:
					return Season.PermanentSummer;
				case Season.PermanentWinter:
					return Season.PermanentWinter;
				default:
					return Season.Undefined;
			}
		}
	}
}
