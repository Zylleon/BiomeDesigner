using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;

namespace ZMapDesigner
{
    public class BiomeWorker_NoDefaultSpawn : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {

            return -100;
        }
    }
}