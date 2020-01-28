using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using RimWorld.Planet;

namespace ZMapDesigner.Gensteps_Vanilla
{
    public class GenStep_ZMDRocksFromGrid : GenStep
    {
        private class RoofThreshold
        {
            public RoofDef roofDef;

            public float minGridVal;
        }

        private float maxMineableValue = 3.40282347E+38f;

        private const int MinRoofedCellsPerGroup = 20;

        public override int SeedPart
        {
            get
            {
                return 1282318295;
            }
        }

        /*
         * // Currently using the vanilla one in GenStep_RocksFromGrid. This may be necessary to change later.
         * 
        public static ThingDef RockDefAt(IntVec3 c)
        {
            ThingDef thingDef = null;
            float num = -999999f;
            for (int i = 0; i < RockNoises.rockNoises.Count; i++)
            {
                float value = RockNoises.rockNoises[i].noise.GetValue(c);
                if (value > num)
                {
                    thingDef = RockNoises.rockNoises[i].rockDef;
                    num = value;
                }
            }
            if (thingDef == null)
            {
                Log.ErrorOnce("Did not get rock def to generate at " + c, 50812, false);
                thingDef = ThingDefOf.Sandstone;
            }
            return thingDef;
        }
        */


        public override void Generate(Map map, GenStepParams parms)
        {

            MountainSettings settings = map.Biome.GetModExtension<ZMDBiomeModExtension>().biomeMapSettings.mountainSettings;

            //if (map.TileInfo.WaterCovered)
            //{
            //    return;
            //}
            map.regionAndRoomUpdater.Enabled = false;
            float mountainThreshhold = 0.7f;

            //float hillTuning = settings.elevationTuning;


            List<GenStep_ZMDRocksFromGrid.RoofThreshold> list = new List<GenStep_ZMDRocksFromGrid.RoofThreshold>();
            list.Add(new GenStep_ZMDRocksFromGrid.RoofThreshold
            {
                roofDef = RoofDefOf.RoofRockThick,
                minGridVal = mountainThreshhold * 1.14f
            });
            list.Add(new GenStep_ZMDRocksFromGrid.RoofThreshold
            {
                roofDef = RoofDefOf.RoofRockThin,
                minGridVal = mountainThreshhold * 1.04f
            });
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid caves = MapGenerator.Caves;
            foreach (IntVec3 current in map.AllCells)
            {
                //float elev = elevation[current] * hillTuning;
                float elev = elevation[current];
                if (elev > mountainThreshhold)
                {
                    if (caves[current] <= 0f)
                    {
                        ThingDef def = GenStep_RocksFromGrid.RockDefAt(current);
                        GenSpawn.Spawn(def, current, map, WipeMode.Vanish);
                    }
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (elev > list[i].minGridVal)
                        {
                            map.roofGrid.SetRoof(current, list[i].roofDef);
                            break;
                        }
                    }
                }
            }
            BoolGrid visited = new BoolGrid(map);
            List<IntVec3> toRemove = new List<IntVec3>();
            foreach (IntVec3 current2 in map.AllCells)
            {
                if (!visited[current2])
                {
                    if (this.IsNaturalRoofAt(current2, map))
                    {
                        toRemove.Clear();
                        map.floodFiller.FloodFill(current2, (IntVec3 x) => this.IsNaturalRoofAt(x, map), delegate (IntVec3 x)
                        {
                            visited[x] = true;
                            toRemove.Add(x);
                        }, 2147483647, false, null);
                        if (toRemove.Count < MinRoofedCellsPerGroup)
                        {
                            for (int j = 0; j < toRemove.Count; j++)
                            {
                                map.roofGrid.SetRoof(toRemove[j], null);
                            }
                        }
                    }
                }
            }
            GenStep_ScatterLumpsMineable genStep_ScatterLumpsMineable = new GenStep_ScatterLumpsMineable();
            genStep_ScatterLumpsMineable.maxValue = this.maxMineableValue;
            float num3 = 10f;
            switch (Find.WorldGrid[map.Tile].hilliness)
            {
                case Hilliness.Flat:
                    num3 = 4f;
                    break;
                case Hilliness.SmallHills:
                    num3 = 8f;
                    break;
                case Hilliness.LargeHills:
                    num3 = 11f;
                    break;
                case Hilliness.Mountainous:
                    num3 = 15f;
                    break;
                case Hilliness.Impassable:
                    num3 = 16f;
                    break;
            }
            genStep_ScatterLumpsMineable.countPer10kCellsRange = new FloatRange(num3, num3);
            genStep_ScatterLumpsMineable.Generate(map, parms);
            map.regionAndRoomUpdater.Enabled = true;
        }

        private bool IsNaturalRoofAt(IntVec3 c, Map map)
        {
            return c.Roofed(map) && c.GetRoof(map).isNatural;
        }
    }
}