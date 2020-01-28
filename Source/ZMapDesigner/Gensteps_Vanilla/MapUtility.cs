using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace ZMapDesigner.Gensteps_Vanilla
{
    public static class MapUtility
    {
        public static TerrainDef TerrainFrom(IntVec3 c, Map map, float elevation, float fertility, RiverMaker river, bool preferSolid)
        {
            float gravelThreshhold = 0.55f;
            float stoneThreshhold = 0.61f;

            MountainSettings settings = map.Biome.GetModExtension<ZMDBiomeModExtension>().biomeMapSettings.mountainSettings;

            //elevation *= settings.elevationTuning;

            TerrainDef riverTerrain = null;
            if (river != null)
            {
                riverTerrain = river.TerrainAt(c, true);
            }
            if (riverTerrain == null && preferSolid)                        // rough stone under mountains, if not river
            {
                return GenStep_RocksFromGrid.RockDefAt(c).building.naturalTerrain;
            }
            TerrainDef terrainDef2 = BeachMaker.BeachTerrainAt(c, map.Biome);
            if (terrainDef2 == TerrainDefOf.WaterOceanDeep)                 // deep ocean from beachmaker
            {
                return terrainDef2;
            }
            if (riverTerrain != null && riverTerrain.IsRiver)               // moving water from rivermaker
            {
                return riverTerrain;
            }
            if (terrainDef2 != null)                                        // Beachmaker (not deep ocean)
            {
                return terrainDef2;
            }
            if (riverTerrain != null)                                       // rivermaker
            {
                return riverTerrain;
            }
            for (int i = 0; i < map.Biome.terrainPatchMakers.Count; i++)    // terrainPatchMakers from xml BiomeDef
            {
                terrainDef2 = map.Biome.terrainPatchMakers[i].TerrainAt(c, map, fertility);
                if (terrainDef2 != null)
                {
                    return terrainDef2;
                }
            }
            if (settings.allowStoneTerrain)                                  // stone around mountains
            {
                if (elevation >= stoneThreshhold)
                {
                    return GenStep_RocksFromGrid.RockDefAt(c).building.naturalTerrain;
                }
            }
            if (settings.allowGravel)                                        // gravel around mountains
            {
                if (elevation > gravelThreshhold)
                {
                    return TerrainDefOf.Gravel;
                }
            }
           
            
            terrainDef2 = TerrainThreshold.TerrainAtValue(map.Biome.terrainsByFertility, fertility);
            if (terrainDef2 != null)                                        // terrains by fertility
            {
                return terrainDef2;
            }

            return TerrainDefOf.Sand;                                       // default to sand if all else fails
        }




    }
}
