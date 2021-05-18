using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
                    if (!(GenLocalDate.DayPercent(this) < ext.growingHours.min))
                    {
                        return GenLocalDate.DayPercent(this) > ext.growingHours.max;
                    }
                    return true;
                }
                return false;
            }
        }

        // If a plant has a max harvest growth, make it harvestable if it hasn't grown too much.
        public override bool HarvestableNow
        {
            get
            {
                DM_ModExtension ext = def.GetModExtension<DM_ModExtension>();
                if ((!def.HasModExtension<DM_ModExtension>() || ext.harvestMaxGrowth == null) && def.plant.Harvestable)
                {
                    return growthInt > def.plant.harvestMinGrowth;
                }
                if (def.plant.Harvestable && ext.harvestMaxGrowth != null)
                {
                    return growthInt < ext.harvestMaxGrowth;
                }
                return false;
            }
        }

        // If a plant has a max harvest growth, make it inedible when outside harvest.
        public override bool IngestibleNow
        {
            get
            {
                if (!base.IngestibleNow)
                {
                    return false;
                }
                if (def.plant.IsTree)
                {
                    return true;
                }
                if (def.HasModExtension<DM_ModExtension>())
                {
                    DM_ModExtension ext = def.GetModExtension<DM_ModExtension>();
                    if (ext.harvestMaxGrowth != null && (growthInt > ext?.harvestMaxGrowth))
                    {
                        return false;
                    }
                }
                if (growthInt < def.plant.harvestMinGrowth)
                {
                    return false;
                }
                if (LeaflessNow)
                {
                    return false;
                }
                if (base.Spawned && base.Position.GetSnowDepth(base.Map) > def.hideAtSnowDepth)
                {
                    return false;
                }
                return true;
            }
        }

        // If a plant has a max harvest growth, reduce yield the older it gets.
        public override int YieldNow()
        {
            if (!CanYieldNow())
            {
                return 0;
            }
            float harvestYield = def.plant.harvestYield;
            float num;
            DM_ModExtension ext = def.GetModExtension<DM_ModExtension>();
            if (ext != null && ext.harvestMaxGrowth != null)
            {
                num = Mathf.InverseLerp((float)ext.harvestMaxGrowth, def.plant.harvestMinGrowth, growthInt);
            }
            else
            {
                num = Mathf.InverseLerp(def.plant.harvestMinGrowth, 1f, growthInt);
            }
            num = 0.5f + num * 0.5f;
            return GenMath.RoundRandom(harvestYield * num * Mathf.Lerp(0.5f, 1f, (float)HitPoints / (float)base.MaxHitPoints) * Find.Storyteller.difficultyValues.cropYieldFactor);
        }

    }
}
