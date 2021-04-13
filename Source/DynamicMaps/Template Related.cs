using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicMaps
{
	public class NoBiomeWorker : BiomeWorker
	{
		public override float GetScore(Tile tile, int tileID)
		{
			return -100f;
		}
	}
}
