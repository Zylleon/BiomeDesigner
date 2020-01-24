using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ZMapDesigner
{
    public class ZMDBiomeModExtension : DefModExtension
    {
        public WorldMapSettings worldMapSettings;
        public BiomeMapSettings biomeMapSettings;


    }

   
    
    public class BiomeMapSettings
    {
        // uses vanilla defaults if not specified
        public MountainSettings mountainSettings;

        // allows beaches if true. Disables beaches if false. Never forces beaches.
        public bool coast = true;

        // true = force caves, false = forbid caves, null = vanilla behaviour. Caves may not actually show up on maps with insufficient rock.
        public bool? caves = null;


        // TODO: implement the rest
        public bool deepOre = true;
        public bool geysers = true;
        public bool mountains = true;
        public bool mountainOre = true;
        public bool ruins = true;
        public bool shrines = true;
        public bool rockChunks = true;
    }


    public class MountainSettings
    {
        public bool allowMountains = true;          // NOT IMPLEMENTED yet

        public bool allowStoneTerrain = true;       // allows rough stone ground around mountains
        public bool allowGravel = true;             // allows gravel around mountains
        public float frequency = 0.021f;            // higher numbers = more small mountains
        public float lacunarity = 2.0f;             // Lower = smooth, higher = rough. Valid values 0 to 9.9
    }


    public class WorldMapSettings
    {
        public float tempMin = 0f;
        public float tempMax = 999f;

        public float elevationMin = -9999f;
        public float elevationMax = 9999f;

        public float rainfallMin = -9999f;
        public float rainfallMax = 9999f;

        public float swampMin = -999f;
        public float swampMax = 999f;

        public Hilliness hillinessMin = Hilliness.Flat;
        public Hilliness hillinessMax = Hilliness.Impassable;

        public int patchSize = 3;

        public bool AllowedOn(Tile tile)
        {
            bool valid = true;

            if(tile.temperature < tempMin || tile.temperature > tempMax )
            {
                return false;
            }
            if (tile.elevation < elevationMin || tile.elevation > elevationMax)
            {
                return false;
            }
            if (tile.rainfall < rainfallMin || tile.rainfall > rainfallMax)
            {
                return false;
            }
            if (tile.swampiness < swampMin || tile.swampiness > swampMax)
            {
                return false;
            }
            if (tile.hilliness < hillinessMin || tile.hilliness > hillinessMax)
            {
                return false;
            }

            return valid;
            
        }

        public int commonality = 10;

        public int NumberOfTiles()
        {
            if(patchSize == 0)
            {
                return 1;
            }

            return Rand.Range(12 * patchSize * patchSize, 50 * patchSize * patchSize);
        }
    }

    //    Average patch size at different levels
        //1
        //25
        //100
        //225
        //400
        //625
        //900
        //1225


}
