using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DynamicMaps
{
	public class DM_ModExtension : DefModExtension
	{
		public string semiMaturePath;
		public Season season;
		public float commonality;
		public float semiMatureAt = 0.5f;
		public float matureAt = 0.8f;
		public float minRainfall;
		public float maxRainfall;
		public float minTemperature;
		public float maxTemperature;
		public bool spawnInPreviousSeason;
		public bool spawnInNextSeason;
		public bool needsRest = true;
		public FloatRange growingHours = new FloatRange(0.25f, 0.8f);
	}
}
