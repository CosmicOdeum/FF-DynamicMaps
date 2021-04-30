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
        public string SemiMaturePath;
		public DM_Season Season;
	}
	public enum DM_Season
	{
		Undefined,
		Spring,
		Summer,
		Autumn,
		Winter
	}
}
