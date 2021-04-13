using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DynamicMaps
{

    [HarmonyPatch(typeof(WeatherDecider), "CurrentWeatherCommonality")]
    internal static class RainPatch
    {
        internal static void Postfix(ref float __result, Map ___map, WeatherDef weather)
        {
            BiomeDef biome = ___map.Biome;
            if (weather.rainRate > 0.1f)
            {
                List<int> neighbors = new List<int>();
                int i = 0;
                int tileID = ___map.Tile;
                WorldGrid grid = Find.WorldGrid;
                grid.GetTileNeighbors(tileID, neighbors);
                float oceanNeighbors = 0;
                for (int count = neighbors.Count; i < count; i++)
                {
                    if (grid[neighbors[i]].biome == BiomeDefOf.Ocean)
                    {
                        oceanNeighbors += 1;
                    }
                }
                __result += (0.1f * oceanNeighbors);
                __result *= oceanNeighbors;
            }
        }
    }
}
