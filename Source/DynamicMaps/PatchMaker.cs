using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace DynamicMaps
{
    class KEP_Patchmaker : GenStep
	{
		public TerrainDef terrainDef;

		public FloatRange patchesPer10kCellsRange;

		public FloatRange patchSizeRange;

		public override int SeedPart => 1370184742;

		public override void Generate(Map map, GenStepParams parms)
		{
			terrainDef = TerrainDef.Named("sand");
			int num = Mathf.RoundToInt((float)map.Area / 10000f * patchesPer10kCellsRange.RandomInRange);
			for (int i = 0; i < num; i++)
			{
				float randomInRange = patchSizeRange.RandomInRange;
				IntVec3 a = CellFinder.RandomCell(map);
				foreach (IntVec3 item in GenRadial.RadialPatternInRadius(randomInRange / 2f))
				{
					IntVec3 c = a + item;
					if (c.InBounds(map))
					{
						map.terrainGrid.SetTerrain(c, terrainDef);
					}
				}
			}
		}
	}
}
