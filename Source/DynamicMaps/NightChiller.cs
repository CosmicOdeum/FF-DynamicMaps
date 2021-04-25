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

namespace DynamicMaps
{
    [HarmonyPatch(typeof(GenTemperature), "OffsetFromSunCycle")]
    internal static class NightChiller
    {
        internal static bool Prefix(ref float __result, int absTick, int tile)
        {
            Tile tile2 = Find.WorldGrid[tile];
            float airMoisture = 1 - ((2000 - tile2.rainfall) / 1000);
            float num = GenDate.DayPercent(absTick, Find.WorldGrid.LongLatOf(tile).x);
            __result =  (Mathf.Cos((float)Math.PI * 2f * (num + 0.32f)) * 7f) / 10f;
            return false;
        }
    }
}
