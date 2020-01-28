using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Noise;
using RimWorld.Planet;

namespace ZMapDesigner.Gensteps_Vanilla
{
    public class GenStep_ZMDElevationFertility : GenStep
    {
        private const float ElevationFreq = 0.021f;

        private const float FertilityFreq = 0.021f;

        private const float EdgeMountainSpan = 0.42f;

        public override int SeedPart
        {
            get
            {
                return 1404712656;
            }
        }

        public override void Generate(Map map, GenStepParams parms)
        {
            MountainSettings settings = map.Biome.GetModExtension<ZMDBiomeModExtension>().biomeMapSettings.mountainSettings;

            NoiseRenderer.renderSize = new IntVec2(map.Size.x, map.Size.z);
            ModuleBase moduleBase = new Perlin(settings.frequency, settings.lacunarity, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
            moduleBase = new ScaleBias(0.5, 0.5, moduleBase);
            NoiseDebugUI.StoreNoiseRender(moduleBase, "elev base");
            float hillTuning = 1f;
            switch (map.TileInfo.hilliness)
            {
                case Hilliness.Flat:
                    hillTuning = MapGenTuning.ElevationFactorFlat;
                    break;
                case Hilliness.SmallHills:
                    hillTuning = MapGenTuning.ElevationFactorSmallHills;
                    break;
                case Hilliness.LargeHills:
                    hillTuning = MapGenTuning.ElevationFactorLargeHills;
                    break;
                case Hilliness.Mountainous:
                    hillTuning = MapGenTuning.ElevationFactorMountains;
                    break;
                case Hilliness.Impassable:
                    hillTuning = MapGenTuning.ElevationFactorImpassableMountains;
                    break;
            }


            moduleBase = new Multiply(moduleBase, new Const((double)hillTuning));
            NoiseDebugUI.StoreNoiseRender(moduleBase, "elev world-factored");
            if (map.TileInfo.hilliness == Hilliness.Mountainous || map.TileInfo.hilliness == Hilliness.Impassable)
            {
                ModuleBase moduleBase2 = new DistFromAxis((float)map.Size.x * EdgeMountainSpan);
                moduleBase2 = new Clamp(0.0, 1.0, moduleBase2);
                moduleBase2 = new Invert(moduleBase2);
                moduleBase2 = new ScaleBias(1.0, 1.0, moduleBase2);
                Rot4 random;
                do
                {
                    random = Rot4.Random;
                }
                while (random == Find.World.CoastDirectionAt(map.Tile));
                if (random == Rot4.North)
                {
                    moduleBase2 = new Rotate(0.0, 90.0, 0.0, moduleBase2);
                    moduleBase2 = new Translate(0.0, 0.0, (double)(-(double)map.Size.z), moduleBase2);
                }
                else if (random == Rot4.East)
                {
                    moduleBase2 = new Translate((double)(-(double)map.Size.x), 0.0, 0.0, moduleBase2);
                }
                else if (random == Rot4.South)
                {
                    moduleBase2 = new Rotate(0.0, 90.0, 0.0, moduleBase2);
                }
                else if (random == Rot4.West)
                {
                }
                NoiseDebugUI.StoreNoiseRender(moduleBase2, "mountain");
                moduleBase = new Add(moduleBase, moduleBase2);
                NoiseDebugUI.StoreNoiseRender(moduleBase, "elev + mountain");
            }

            //float b = (!map.TileInfo.WaterCovered) ? 3.40282347E+38f : 0f;
            float b = 3.40282347E+38f;
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            float elevationTuning = settings.elevationTuning;
            foreach (IntVec3 current in map.AllCells)
            {
                elevation[current] = elevationTuning * Mathf.Min(moduleBase.GetValue(current), b);
            }
            
            //fertility
            ModuleBase moduleBase3 = new Perlin(0.020999999716877937, 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
            moduleBase3 = new ScaleBias(0.5, 0.5, moduleBase3);
            NoiseDebugUI.StoreNoiseRender(moduleBase3, "noiseFert base");
            MapGenFloatGrid fertility = MapGenerator.Fertility;
            foreach (IntVec3 current2 in map.AllCells)
            {
                fertility[current2] = moduleBase3.GetValue(current2);
            }
        }
    }
}
