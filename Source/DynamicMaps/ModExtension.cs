﻿using RimWorld;
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
		public bool spawnInPreviousSeason;
		public bool spawnInNextSeason;
	}
}
