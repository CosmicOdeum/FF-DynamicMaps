using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DynamicMaps
{
	[StaticConstructorOnStartup]
	public class DM_Plant : Plant
	{
		public Graphic semiMatureGraphic;
		public override Graphic Graphic
		{
			get
			{
				if (!def.HasModExtension<DM_ModExtension>())
					return base.Graphic;
				if (LifeStage == PlantLifeStage.Sowing)
					return base.Graphic;
				if (def.plant.leaflessGraphic != null && LeaflessNow && (!sown || !HarvestableNow))
					return base.Graphic;
				DM_ModExtension ext = def.GetModExtension<DM_ModExtension>();
				Graphic graphic = def.graphic;
                if (def.plant.immatureGraphic != null)
                {
                    graphic = def.plant.immatureGraphic;
                }
                if (ext.semiMaturePath != null && Growth > ext.semiMatureAt)
                {
                    graphic = GraphicDatabase.Get(def.graphicData.graphicClass, ext.semiMaturePath, def.graphic.Shader, def.graphicData.drawSize, def.graphicData.color, base.Graphic.colorTwo);
                }
                if (def.plant.immatureGraphic != null && Growth > ext.matureAt)
                {
                    graphic = def.graphic;
                }
                return graphic;
			}
		}
        protected override bool Resting => isResting;
        bool isResting
        {
            get
            {
                if (!def.HasModExtension<DM_ModExtension>())
                    return base.Resting;
                DM_ModExtension ext = def.GetModExtension<DM_ModExtension>();
                if (ext.needsRest)
                {
                    if (!(GenLocalDate.DayPercent(this) < ext.growingHours.max))
                    {
                        return GenLocalDate.DayPercent(this) > ext.growingHours.min;
                    }
                    return true;
                }
                return false;
            }
        }
    }
}
