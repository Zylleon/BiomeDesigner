using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse;


namespace ZMapDesigner
{
    public static class GenWorldMapBiomes
    {
        public static void PlaceBiome(BiomeDef biome, WorldMapSettings settings)
        {
            WorldGrid worldGrid = Find.WorldGrid;

            List<int> usedTiles = new List<int>();
            for (int i = 0; i < 500 && usedTiles.Count() < settings.NumberOfTiles(); i++)
            {
                int tileID = Rand.Range(0, worldGrid.TilesCount);

                Tile tile = worldGrid[tileID];


                // TODO: Make this do stuff. Properly.
                if (settings.AllowedOn(tile))
                {
                    List<int> tmpTiles = new List<int>();
                    tile.biome = biome;
                    usedTiles.Add(tileID);
                    while (usedTiles.Count() < settings.NumberOfTiles())
                    {
                        tmpTiles.Clear();

                        worldGrid.GetTileNeighbors(tileID, tmpTiles);

                        foreach (int id in tmpTiles)
                        {
                            //if (settings.AllowedOn)




                        }


                    }
                }
            }


        }


        public static void PlaceBiome_New(BiomeDef biome, WorldMapSettings settings)
        {
            WorldGrid worldGrid = Find.WorldGrid;

            List<int> usedTiles = new List<int>();
            List<int> edgeTiles = new List<int>();

            int totalCount = settings.NumberOfTiles();

            List<int> tmpTiles = new List<int>();

            for (int i = 0; i < settings.commonality * 10; i++)
            {
                // try to find a starting point
                int tileID = Rand.Range(0, worldGrid.TilesCount);
                Tile tile = worldGrid[tileID];

                usedTiles.Clear();
                edgeTiles.Clear();

                // if biome is allowed on that tile, start patch
                if (settings.AllowedOn(tile))
                {
                    tile.biome = biome;
                    usedTiles.Add(tileID);
                    edgeTiles.Add(tileID);

                    while (edgeTiles.Count > 0 && usedTiles.Count() < settings.NumberOfTiles())
                    {
                        tmpTiles.Clear();

                        tileID = edgeTiles.RandomElement();

                        worldGrid.GetTileNeighbors(tileID, tmpTiles);

                        tmpTiles.RemoveAll(t => worldGrid[t].biome == biome || !settings.AllowedOn(worldGrid[t]));            // remove neighbors that already have biome, or that aren't valid targets
                        
                        if (tmpTiles.Count > 0)
                        {
                            foreach (int id in tmpTiles)
                            {
                                tile = worldGrid[id];
                                // this if statement is probably unnecessary
                                if (settings.AllowedOn(tile))               
                                {
                                    tile.biome = biome;
                                    usedTiles.Add(id);
                                    edgeTiles.Add(id);
                                }
                            }
                        }
                        else            // remove tile, it's no longer an edge tile
                        {
                            edgeTiles.Remove(tileID);
                        }

                    }
                }
            }
        }
    }
}
