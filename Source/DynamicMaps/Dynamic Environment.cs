using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;
using Verse.Noise;

namespace DynamicMaps
{
    [StaticConstructorOnStartup]
    public static class starter
    {
        static starter()
        {
            new Harmony("FF.DynamicMaps").PatchAll();
        }
    }
    //[HarmonyPatch(typeof(MouseoverReadout), "MouseoverReadoutOnGUI")]
    //internal static class FertilityGUIPatch
    //{
    //    internal static void Prefix(string ___cachedTerrainString, TerrainDef ___cachedTerrain)
    //    {
    //        Vector2 vec2 = new Vector2(15f, 65f);
    //        FertilityGrid fertilityGrid = Find.CurrentMap.fertilityGrid;
    //        IntVec3 c = UI.MouseCell();
    //        TerrainDef terrain = c.GetTerrain(Find.CurrentMap);
    //        string speedString = (13f / (terrain.pathCost + 13f)).ToStringPercent();
    //        string t = ((double)terrain.fertility > 0.0001) ? (" " + "FertShort".TranslateSimple() + " " + fertilityGrid.FertilityAt(c).ToStringPercent()) : "";
    //        string cachedTerrainString = terrain.LabelCap + ((terrain.passability != Traversability.Impassable) ? (" (" + "WalkSpeed".Translate(speedString) + t + ")") : ((TaggedString)null));
    //        Log.Error(t);
    //        ___cachedTerrain = terrain;
    //        float num = 19f;
    //        Rect rect2 = new Rect(vec2.x, (float)UI.screenHeight - vec2.y - num, 999f, 999f);
    //        Widgets.Label(rect2, cachedTerrainString);
    //    }

    //}

    //[HarmonyPatch(typeof(FertilityGrid), "CalculateFertilityAt")]
    //internal static class FertilityPatch
    //{
    //    internal static void Postfix(ref float __result, IntVec3 loc, Map ___map)
    //    {
    //        //Thing edifice = loc.GetEdifice(___map);
    //        //if (edifice != null && edifice.def.AffectsFertility)
    //        //{
    //        //    fertility = edifice.def.fertility;
    //        //}
    //        //__result *= 2;
    //    }
    //}

    [HarmonyPatch(typeof(PlantUtility), nameof(PlantUtility.CanEverPlantAt_NewTemp))]
    internal static class CanPlantOnMap
    {
        internal static void Postfix(ref bool __result, ThingDef plantDef, Map map)
        { 
            if (__result && plantDef.HasModExtension<DM_ModExtension>())
            {
                DM_ModExtension ext = plantDef.GetModExtension<DM_ModExtension>();
                Tile tile = Find.WorldGrid[map.Tile];
                if (tile.rainfall < ext.minRainfall || tile.rainfall > ext.maxRainfall || tile.temperature < ext.minTemperature || tile.temperature > ext.maxTemperature)
                    __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(WildPlantSpawner), "GetBaseDesiredPlantsCountAt")]
    internal static class PlantDensityPatch
    {
        public static Dictionary<int, float> cellPerlin = new Dictionary<int, float>();
        //public static Dictionary<int, enum> cellMoisture = new Dictionary<int, enum>();
        public static ModuleBase PerlinNoise = null;
        public enum cellMoisture
        {
            ExtremelyDry,
            VeryDry,
            Dry,
            Moist,
            Wet,
            VertWet,
            Submerged,
            DeeplySubmerged

        }
        internal static void Postfix(ref float __result, IntVec3 c, Map ___map)
        {
            float result = 0;
            if (!cellPerlin.ContainsKey(CellIndicesUtility.CellToIndex(c, c.x + c.y)))
            {
                var terrain = c.GetTerrain(___map);
                float fertility = terrain.fertility;
                var map = ___map;
                float temp = map.TileInfo.temperature / 15;
                float rainfall = map.TileInfo.rainfall / 1000;
                int perlinSeed = Find.World.info.Seed;
                float perlinNoiseValue = 0;
                PerlinNoise = new Perlin(0.1, 10, 0.6, 12, perlinSeed, QualityMode.Low);
                perlinNoiseValue = PerlinNoise.GetValue(c);
                if (temp < 0.5)
                    temp = 0.5f;
                if (temp > 2.6)
                    temp = 2.6f;
                if (rainfall < 0.5)
                    rainfall = 0.5f;
                //Log.Error("" + map.Biome.plantDensity);
                result = ((perlinNoiseValue + 0.8f + fertility) / 2) * ((( 1 - map.Biome.plantDensity) + temp + rainfall) / 3);
                if (fertility > 1.1f)
                    result *= 2;
                else if (fertility < 0.5f)
                    result /= 4;
                if (!cellPerlin.ContainsKey(CellIndicesUtility.CellToIndex(c, c.x + c.y)))
                    cellPerlin.Add(CellIndicesUtility.CellToIndex(c, c.x + c.y), result);
                __result = result;
                return;
            }
            else
            {
                result = cellPerlin[CellIndicesUtility.CellToIndex(c, c.x + c.y)];
                __result = result;
                return;
            }

        }
    }
}
