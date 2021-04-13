using HarmonyLib;
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

namespace DynamicMapGen
{
    [HarmonyPatch(typeof(WildPlantSpawner), "CalculatePlantsWhichCanGrowAt")]
    internal static class UniversalPlants
    {
        internal static void Postfix(IntVec3 c, Map ___map, List<ThingDef> outPlants)
        {
            ThingDef thingDef = ThingDef.Named("DM_GravekTwister");
            if (!thingDef.CanEverPlantAt_NewTemp(c, ___map))
            {
                return;
            }
            outPlants.Add(thingDef);
        }
    }
    [HarmonyPatch(typeof(WildPlantSpawner), "GetCommonalityOfPlant")]
    internal static class UniversalPlantsCommonality
    {
        internal static void Postfix(ref float __result, ThingDef plant)
        {
            if (plant.defName == "DM_GravekTwister")
                __result = 5;
        }
    }
}
